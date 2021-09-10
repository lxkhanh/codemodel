//-----------------------------------------------------------------------
// <copyright file="JStaticJavaFile.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace CodeModel.fmt
{
    public sealed class JStaticJavaFile : JResourceFile
    {

        private readonly JPackage pkg;
        private readonly string className;
        private readonly Uri source;
        private readonly JStaticClass clazz;
        private readonly LineFilter filter;

        public JStaticJavaFile(JPackage _pkg, string className, string _resourceName)
            : this(_pkg, className, new Uri(FindResouceByName(SecureLoader.getClassClassLoader(typeof(JStaticJavaFile)), _resourceName)), null)
        {
        }

        public JStaticJavaFile(JPackage _pkg, string _className, Uri _source, LineFilter _filter)
            : base(_className + ".java")
        {
            if (_source == null)
            {
                throw new System.NullReferenceException();
            }
            this.pkg = _pkg;
            this.clazz = new JStaticClass(this);
            this.className = _className;
            this.source = _source;
            this.filter = _filter;
        }

        /// <summary>
        /// Returns a class object that represents a statically generated code. 
        /// </summary>
        public JClass JClass
        {
            get
            {
                return clazz;
            }
        }

        public override bool isResource
        {
            get
            {
                return false;
            }
        }
    
        public override void build(System.IO.Stream os)
        {
            WebClient myWebClient = new WebClient();           
            // Open a stream to point to the data stream coming from the Web resource.
            Stream @is = myWebClient.OpenRead(source.AbsoluteUri);
            System.IO.StreamReader r = new System.IO.StreamReader(@is);
            System.IO.StreamWriter w = new System.IO.StreamWriter(os);
            LineFilter filter = createLineFilter();
            int lineNumber = 1;

            try
            {
                string line;
                while (!string.ReferenceEquals((line = r.ReadLine()), null))
                {
                    line = filter.process(line);
                    if (!string.ReferenceEquals(line, null))
                    {
                        w.WriteLine(line);
                    }
                    lineNumber++;
                }
            }
            catch (Exception e)
            {
                throw new Exception("unable to process " + source + " line:" + lineNumber + "\n" + e.Message);
            }

            w.Close();
            r.Close();
        }

        /// <summary>
        /// Creates a <seealso cref="LineFilter"/>.
        /// <para>
        /// A derived class can override this method to process
        /// the contents of the source file.
        /// </para>
        /// </summary>
        private LineFilter createLineFilter()
        {
            // this filter replaces the package declaration.
            LineFilter f = new LineFilterAnonymousInnerClass(this);
            if (filter != null)
            {
                return new ChainFilter(filter, f);
            }
            else
            {
                return f;
            }
        }

        private class LineFilterAnonymousInnerClass : LineFilter
        {
            private readonly JStaticJavaFile outerInstance;

            public LineFilterAnonymousInnerClass(JStaticJavaFile outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual string process(string line)
            {
                if (!line.StartsWith("package ", StringComparison.Ordinal))
                {
                    return line;
                }

                // replace package decl
                if (outerInstance.pkg.isUnnamed)
                {
                    return null;
                }
                else
                {
                    return "package " + outerInstance.pkg.name + ";";
                }
            }
        }

        /// <summary>
        /// Filter that alters the Java source code.
        /// <para>
        /// By implementing this interface, derived classes
        /// can modify the Java source file before it's written out.
        /// </para>
        /// </summary>
        public interface LineFilter
        {
            /// <param name="line">
            ///      a non-null valid String that corresponds to one line.
            ///      No '\n' included.
            /// @return
            ///      null to strip the line off. Otherwise the returned
            ///      String will be written out. Do not add '\n' at the end
            ///      of this string.
            /// </param>       
            string process(string line);
        }

        /// <summary>
        /// A <seealso cref="LineFilter"/> that combines two <seealso cref="LineFilter"/>s.
        /// </summary>
        public sealed class ChainFilter : LineFilter
        {
            internal readonly LineFilter first, second;
            public ChainFilter(LineFilter first, LineFilter second)
            {
                this.first = first;
                this.second = second;
            }        
            public string process(string line)
            {
                line = first.process(line);
                if (string.ReferenceEquals(line, null))
                {
                    return null;
                }
                return second.process(line);
            }
        }


        private class JStaticClass : JClass
        {
            private readonly JStaticJavaFile outerInstance;
       
            internal readonly JTypeVar[] typeParams;

            internal JStaticClass(JStaticJavaFile outerInstance)
                : base(outerInstance.pkg.owner())
            {
                this.outerInstance = outerInstance;
                // TODO: allow those to be specified
                typeParams = new JTypeVar[0];
            }

            public override string name()
            {
                return outerInstance.className;
            }

            public override string fullName()
            {
                if (outerInstance.pkg.isUnnamed)
                {
                    return outerInstance.className;
                }
                else
                {
                    return outerInstance.pkg.name + '.' + outerInstance.className;
                }
            }

            public override JPackage _package()
            {
                return outerInstance.pkg;
            }

            public override JClass _extends()
            {
                throw new System.NotSupportedException();
            }

            public override IEnumerator<JClass> _implements()
            {
                throw new System.NotSupportedException();
            }

            public override bool isInterface
            {
                get
                {
                    throw new System.NotSupportedException();
                }
            }

            public override bool isAbstract
            {
                get
                {
                    throw new System.NotSupportedException();
                }
            }

            public virtual JTypeVar[] TypeParams()
            {
                return typeParams;
            }

            public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
            {
                return this;
            }            
        }

        public static string FindResouceByName(Assembly assembly, string resourceName)
        {
            string[] names = assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name == resourceName)
                {
                    return name;
                }
            }
            return string.Empty;
        }
    }
}
