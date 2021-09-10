//-----------------------------------------------------------------------
// <copyright file="JPackage.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel
{
    /// <summary>
    /// A Java package.
    /// </summary>
    public sealed class JPackage : JDeclaration, JGenerable, JClassContainer, JAnnotatable, IComparable<JPackage>, JDocCommentable
    {
        /// <summary>
        /// Name of the package.
        /// May be the empty string for the root package.
        /// </summary>
        private string _name;

        private readonly JCodeModel _owner;

        /// <summary>
        /// List of classes contained within this package keyed by their name.
        /// </summary>
        private readonly IDictionary<string, JDefinedClass> _classes = new SortedDictionary<string, JDefinedClass>();

        /// <summary>
        /// List of resources files inside this package.
        /// </summary>
        private readonly ISet<JResourceFile> resources = new HashSet<JResourceFile>();

        /// <summary>
        /// All <seealso cref="JClass"/>s in this package keyed the upper case class name.
        /// 
        /// This field is non-null only on Windows, to detect
        /// "Foo" and "foo" as a collision. 
        /// </summary>
        private readonly IDictionary<string, JDefinedClass> upperCaseClassMap;

        /// <summary>
        /// Lazily created list of package annotations.
        /// </summary>
        private IList<JAnnotationUse> _annotations = null;

        /// <summary>
        /// package javadoc.
        /// </summary>
        private JDocComment jdoc = null;

        /// <summary>
        /// JPackage constructor
        /// </summary>
        /// <param name="name">
        ///        Name of package
        /// </param>
        /// <param name="cw">  The code writer being used to create this package
        /// </param>
        /// <exception cref="IllegalArgumentException">
        ///         If each part of the package name is not a valid identifier </exception>
        internal JPackage(string name, JCodeModel cw)
        {
            this._owner = cw;
            if (name.Equals("."))
            {
                string msg = "Package name . is not allowed";
                throw new System.ArgumentException(msg);
            }

            if (JCodeModel.isCaseSensitiveFileSystem)
            {
                upperCaseClassMap = null;
            }
            else
            {
                upperCaseClassMap = new Dictionary<string, JDefinedClass>();
            }

            this._name = name;
        }


        public JClassContainer parentContainer()
        {
            return parent();
        }

        /// <summary>
        /// Gets the parent package, or null if this class is the root package.
        /// </summary>
        public JPackage parent()
        {
            if (_name.Length == 0)
            {
                return null;
            }

            int idx = _name.LastIndexOf('.');
            return _owner._package(_name.Substring(0, idx));
        }

        public bool isClass
        {
            get
            {
                return false;
            }
        }

        public bool isPackage
        {
            get
            {
                return true;
            }
        }

        public JPackage package
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Add a class to this package.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this class declaration
        /// </param>
        /// <param name="name">
        ///        Name of class to be added to this package
        /// </param>
        /// <returns> Newly generated class
        /// </returns>
        /// <exception cref="JClassAlreadyExistsException">
        ///      When the specified class/interface was already created. </exception>
    
        public JDefinedClass _class(int mods, string name)
        {
            return _class(mods, name, ClassType.CLASS);
        }

        /// <summary>
        /// {@inheritDoc}
        /// @deprecated
        /// </summary>
        public JDefinedClass _class(int mods, string name, bool isInterface)
        {
            return _class(mods, name, isInterface ? ClassType.INTERFACE : ClassType.CLASS);
        }
       
        public JDefinedClass _class(int mods, string name, ClassType classTypeVal)
        {
            if (_classes !=null && _classes.ContainsKey(name))
            {
                throw new JClassAlreadyExistsException(_classes[name]);
            }
            else
            {
                // XXX problems caught in the NC constructor
                JDefinedClass c = new JDefinedClass(this, mods, name, classTypeVal);

                if (upperCaseClassMap != null)
                {
                    JDefinedClass dc = null;
                    if (upperCaseClassMap.ContainsKey(name.ToUpper())) {
                        dc = upperCaseClassMap[name.ToUpper()];
                    }                  
                    if (dc != null)
                    {
                        throw new JClassAlreadyExistsException(dc);
                    }
                    upperCaseClassMap.Add(name.ToUpper(), c);
                }
                _classes.Add(name, c);
                return c;
            }
        }

        /// <summary>
        /// Adds a public class to this package.
        /// </summary>
       
        public JDefinedClass _class(string name)
        {
            return _class(JMod.PUBLIC, name);
        }

        /// <summary>
        /// Gets a reference to the already created <seealso cref="JDefinedClass"/>.
        /// </summary>
        /// <returns> null
        ///      If the class is not yet created. </returns>
        public JDefinedClass _getClass(string name)
        {
            if (_classes!=null && _classes.ContainsKey(name))
            {
                return _classes[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Order is based on the lexicological order of the package name.
        /// </summary>
        public int CompareTo(JPackage that)
        {
            return this._name.CompareTo(that._name);
        }

        /// <summary>
        /// Add an interface to this package.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this interface declaration
        /// </param>
        /// <param name="name">
        ///        Name of interface to be added to this package
        /// </param>
        /// <returns> Newly generated interface </returns>
       
        public JDefinedClass _interface(int mods, string name)
        {
            return _class(mods, name, ClassType.INTERFACE);
        }

        /// <summary>
        /// Adds a public interface to this package.
        /// </summary>      
        public JDefinedClass _interface(string name)
        {
            return _interface(JMod.PUBLIC, name);
        }

        /// <summary>
        /// Add an annotationType Declaration to this package </summary>
        /// <param name="name">
        ///      Name of the annotation Type declaration to be added to this package
        /// @return
        ///      newly created Annotation Type Declaration </param>
        /// <exception cref="JClassAlreadyExistsException">
        ///      When the specified class/interface was already created.
        ///  </exception>       
        public JDefinedClass _annotationTypeDeclaration(string name)
        {
            return _class(JMod.PUBLIC, name, ClassType.ANNOTATION_TYPE_DECL);
        }

        /// <summary>
        /// Add a public enum to this package </summary>
        /// <param name="name">
        ///      Name of the enum to be added to this package
        /// @return
        ///      newly created Enum </param>
        /// <exception cref="JClassAlreadyExistsException">
        ///      When the specified class/interface was already created.
        ///  </exception>       
        public JDefinedClass _enum(string name)
        {
            return _class(JMod.PUBLIC, name, ClassType.ENUM);
        }
        /// <summary>
        /// Adds a new resource file to this package.
        /// </summary>
        public JResourceFile addResourceFile(JResourceFile rsrc)
        {
            resources.Add(rsrc);
            return rsrc;
        }

        /// <summary>
        /// Checks if a resource of the given name exists.
        /// </summary>
        public bool hasResourceFile(string name)
        {
            foreach (JResourceFile r in resources)
            {
                if (r.name().Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Iterates all resource files in this package.
        /// </summary>
        public IEnumerator<JResourceFile> propertyFiles()
        {
            return resources.GetEnumerator();
        }

        /// <summary>
        /// Creates, if necessary, and returns the package javadoc for this
        /// JDefinedClass.
        /// </summary>
        /// <returns> JDocComment containing javadocs for this class </returns>
        public JDocComment javadoc()
        {
            if (jdoc == null)
            {
                jdoc = new JDocComment(_owner);
            }
            return jdoc;
        }

        /// <summary>
        /// Removes a class from this package.
        /// </summary>
        public void remove(JClass c)
        {
            if (c._package() != this)
            {
                throw new System.ArgumentException("the specified class is not a member of this package," + " or it is a referenced class");
            }

            // note that c may not be a member of classes.
            // this happens when someone is trying to remove a non generated class
            if (_classes.ContainsKey(c.name())) {
                _classes.Remove(c.name());
            }            
            if (upperCaseClassMap != null)
            {
                if (upperCaseClassMap.ContainsKey(c.name().ToUpper())) {
                    upperCaseClassMap.Remove(c.name().ToUpper());
                }              
            }
        }

        /// <summary>
        /// Reference a class within this package.
        /// </summary>
      
        public JClass @ref(string name)
        {
            if (name.IndexOf('.') >= 0)
            {
                throw new System.ArgumentException("JClass name contains '.': " + name);
            }

            string n = "";
            if (!isUnnamed)
            {
                n = this._name + '.';
            }
            n += name;

            return _owner.@ref(Type.GetType(n));
        }

        /// <summary>
        /// Gets a reference to a sub package of this package.
        /// </summary>
        public JPackage subPackage(string pkg)
        {
            if (isUnnamed)
            {
                return _owner._package(pkg);
            }
            else
            {
                return _owner._package(_name + '.' + pkg);
            }
        }

        /// <summary>
        /// Returns an iterator that walks the top-level classes defined in this
        /// package.
        /// </summary>
        public IEnumerator<JDefinedClass> classes()
        {
            if (_classes != null && _classes.Values != null) {
                return _classes.Values.GetEnumerator();
            }
            return null;
        }

        /// <summary>
        /// Checks if a given name is already defined as a class/interface
        /// </summary>
        public bool isDefined(string classLocalName)
        {
            IEnumerator<JDefinedClass> itr = classes();
            while (itr.MoveNext())
            {
                if ((itr.Current).name().Equals(classLocalName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if this package is the root, unnamed package.
        /// </summary>
        public bool isUnnamed
        {
            get
            {
                return _name.Length == 0;
            }
        }

        /// <summary>
        /// Get the name of this package
        /// 
        /// @return
        ///		The name of this package, or the empty string if this is the
        ///		null package. For example, this method returns strings like
        ///		<code>"java.lang"</code>
        /// </summary>
        public string name
        {
            get { return _name; }
        }

        /// <summary>
        /// Return the code model root object being used to create this package.
        /// </summary>
        public JCodeModel owner()
        {
            return _owner; 
        }


        public JAnnotationUse annotate(JClass clazz)
        {
            if (isUnnamed)
            {
                throw new System.ArgumentException("the root package cannot be annotated");
            }
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            JAnnotationUse a = new JAnnotationUse(clazz);
            _annotations.Add(a);
            return a;
        }

        public JAnnotationUse annotate(Type clazz) 
        {
            return annotate(owner().@ref(clazz));
        }

        public W annotate2<W, A>(W clazz) where W : JAnnotationWriter<A> where A: Attribute
        {
            return TypedAnnotationWriter<A, W>.create<A, W>(clazz, this);
        }

        public ICollection<JAnnotationUse> annotations()
        {
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            return _annotations;
        }

        /// <summary>
        /// Convert the package name to directory path equivalent
        /// </summary>
        internal DirectoryInfo toPath(DirectoryInfo dir)
        {
            if (name == null)
            {
                return dir;
            }
            return new DirectoryInfo(dir.FullName +  name.Replace('.', System.IO.Path.DirectorySeparatorChar));
        }

        public void declare(JFormatter f)
        {
            if (name.Length != 0)
            {
                f.p("package").p(name).p(';').nl();
            }
        }

        public void generate(JFormatter f)
        {
            f.p(name);
        }
     
        internal void build(CodeWriter src, CodeWriter res)
        {

            // write classes
            if (_classes != null && _classes.Values != null) {
                foreach (JDefinedClass c in _classes.Values)
                {
                    if (c.isHidden)
                    {
                        continue; // don't generate this file
                    }

                    JFormatter f = createJavaSourceFileWriter(src, c.name());
                    f.write(c);
                    f.close();
                }
            }
            
            // write package annotations
            if (_annotations != null || jdoc != null)
            {
                JFormatter f = createJavaSourceFileWriter(src, "package-info");

                if (jdoc != null)
                {
                    f.g((JGenerable)jdoc);
                }

                // TODO: think about importing
                if (_annotations != null)
                {
                    foreach (JAnnotationUse a in _annotations)
                    {
                        f.g(a).nl();
                    }
                }
                f.d(this);

                f.close();
            }

            // write resources
            foreach (JResourceFile rsrc in resources)
            {
                CodeWriter cw = rsrc.isResource ? res : src;
                BufferedStream os = new BufferedStream(cw.openBinary(this, rsrc.name()));
                rsrc.build(os);
                os.Close();
            }
        }

        /*package*/
        internal int countArtifacts()
        {
            int r = 0;
            if (_classes != null && _classes.Values != null) {
                foreach (JDefinedClass c in _classes.Values)
                {
                    if (c.isHidden)
                    {
                        continue; // don't generate this file
                    }
                    r++;
                }
            }        

            if (_annotations != null || jdoc != null)
            {
                r++;
            }

            r += resources.Count;

            return r;
        }
      
        private JFormatter createJavaSourceFileWriter(CodeWriter src, string className)
        {
            TextWriter bw = src.openSource(this, className + ".java");
            return new JFormatter(bw);
        }
    }
}
