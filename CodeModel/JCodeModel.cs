//-----------------------------------------------------------------------
// <copyright file="JCodeModel.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeModel;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;
using CodeModel.writer;

namespace CodeModel
{
    public sealed class JCodeModel
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            NULL = new JNullType(this);
            VOID = new JPrimitiveType(this, "void", typeof(void));
            BOOLEAN = new JPrimitiveType(this, "boolean", typeof(bool));
            BYTE = new JPrimitiveType(this, "byte", typeof(byte));
            SHORT = new JPrimitiveType(this, "short", typeof(short));
            CHAR = new JPrimitiveType(this, "char", typeof(char));
            INT = new JPrimitiveType(this, "int", typeof(int));
            FLOAT = new JPrimitiveType(this, "float", typeof(float));
            LONG = new JPrimitiveType(this, "long", typeof(long));
            DOUBLE = new JPrimitiveType(this, "double", typeof(Double));
        }


        /// <summary>
        /// The packages that this JCodeWriter contains. </summary>        
        private Dictionary<string, JPackage> _packages = new Dictionary<string, JPackage>();

        /// <summary>
        /// All JReferencedClasses are pooled here. </summary>
        private readonly Dictionary<Type, JReferencedClass> refClasses = new Dictionary<Type, JReferencedClass>();


        /// <summary>
        /// Obtains a reference to the special "null" type. </summary>
        public JNullType NULL;
        // primitive types 
        public JPrimitiveType VOID;
        public JPrimitiveType BOOLEAN;
        public JPrimitiveType BYTE;
        public JPrimitiveType SHORT;
        public JPrimitiveType CHAR;
        public JPrimitiveType INT;
        public JPrimitiveType FLOAT;
        public JPrimitiveType LONG;
        public JPrimitiveType DOUBLE;

        /// <summary>
        /// If the flag is true, we will consider two classes "Foo" and "foo"
        /// as a collision.
        /// </summary>
        protected internal static readonly bool isCaseSensitiveFileSystem = FileSystemCaseSensitivity;

        private static bool FileSystemCaseSensitivity
        {
            get
            {
                try
                {
                    // let the system property override, in case the user really
                    // wants to override.
                    string file = Path.GetTempPath() + Guid.NewGuid().ToString().ToLower();
                    File.CreateText(file).Close();
                    bool isCaseInsensitive = File.Exists(file.ToUpper());
                    File.Delete(file);
                    if (isCaseInsensitive)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                }

                // on Unix, it's case sensitive.
                return (System.IO.Path.DirectorySeparatorChar == '/');
            }
        }


        public JCodeModel()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        /// <summary>
        /// Add a package to the list of packages to be generated
        /// </summary>
        /// <param name="name">
        ///        Name of the package. Use "" to indicate the root package.
        /// </param>
        /// <returns> Newly generated package </returns>
        public JPackage _package(string name)
        {
            JPackage p = null;
            if (_packages != null && _packages.ContainsKey(name)) { 
                p = _packages[name];
            }
            if (p == null)
            {
                p = new JPackage(name, this);
                _packages[name] = p;
            }
            return p;
        }

        public JPackage rootPackage()
        {
            return _package("");
        }

        /// <summary>
        /// Returns an iterator that walks the packages defined using this code
        /// writer.
        /// </summary>
        public IEnumerator<JPackage> packages()
        {
            return _packages.Values.GetEnumerator();
        }

        /// <summary>
        /// Creates a new generated class.
        /// </summary>     
        public JDefinedClass _class(string fullyqualifiedName)
        {
            return _class(fullyqualifiedName, ClassType.CLASS);
        }

        /// <summary>
        /// Creates a dummy, unknown <seealso cref="JClass"/> that represents a given name.
        /// 
        /// <para>
        /// This method is useful when the code generation needs to include the user-specified
        /// class that may or may not exist, and only thing known about it is a class name.
        /// </para>
        /// </summary>
        public JClass directClass(string name)
        {
            return new JDirectClass(this, name);
        }

        /// <summary>
        /// Creates a new generated class.
        /// </summary>    
        public JDefinedClass _class(int mods, string fullyqualifiedName, ClassType t)
        {
            int idx = fullyqualifiedName.LastIndexOf('.');
            if (idx < 0)
            {
                return rootPackage()._class(fullyqualifiedName);
            }
            else
            {
                return _package(fullyqualifiedName.Substring(0, idx))._class(mods, fullyqualifiedName.Substring(idx + 1), t);
            }
        }

        /// <summary>
        /// Creates a new generated class.
        /// </summary>
        /// <exception cref="JClassAlreadyExistsException">
        ///      When the specified class/interface was already created. </exception>
        public JDefinedClass _class(string fullyqualifiedName, ClassType t)
        {
            return _class(JMod.PUBLIC, fullyqualifiedName, t);
        }

        /// <summary>
        /// Gets a reference to the already created generated class.
        /// </summary>
        /// <returns> null
        ///      If the class is not yet created. </returns>
        /// <seealso cref= JPackage#_getClass(String) </seealso>
        public JDefinedClass _getClass(string fullyQualifiedName)
        {
            int idx = fullyQualifiedName.LastIndexOf('.');
            if (idx < 0)
            {
                return rootPackage()._getClass(fullyQualifiedName);
            }
            else
            {
                return _package(fullyQualifiedName.Substring(0, idx))._getClass(fullyQualifiedName.Substring(idx + 1));
            }
        }

        /// <summary>
        /// Creates a new anonymous class.
        /// 
        /// @deprecated
        ///      The naming convention doesn't match the rest of the CodeModel.
        ///      Use <seealso cref="#anonymousClass(JClass)"/> instead.
        /// </summary>
        public JDefinedClass newAnonymousClass(JClass baseType)
        {
            return new JAnonymousClass(baseType);
        }

        /// <summary>
        /// Creates a new anonymous class.
        /// </summary>
        public JDefinedClass anonymousClass(JClass baseType)
        {
            return new JAnonymousClass(baseType);
        }

        public JDefinedClass anonymousClass(Type baseType)
        {
            return anonymousClass(@ref(baseType));
        }

        /// <summary>
        /// Generates Java source code.
        /// A convenience method for <code>build(destDir,destDir,System.out)</code>.
        /// 
        /// @param	destDir
        ///		source files are generated into this directory. </summary>
        /// <param name="status">
        ///      if non-null, progress indication will be sent to this stream. </param>       
        public void build(DirectoryInfo destDir, StreamWriter status)
        {
            build(destDir, destDir, status);
        }

        /// <summary>
        /// Generates Java source code.
        /// A convenience method that calls <seealso cref="#build(CodeWriter,CodeWriter)"/>.
        /// 
        /// @param	srcDir
        ///		Java source files are generated into this directory.
        /// @param	resourceDir
        ///		Other resource files are generated into this directory. </summary>
        /// <param name="status">
        ///      if non-null, progress indication will be sent to this stream. </param>       
        public void build(DirectoryInfo srcDir, DirectoryInfo resourceDir, StreamWriter status)
        {
            CodeWriter src = new FileCodeWriter(srcDir);
            CodeWriter res = new FileCodeWriter(resourceDir);
            if (status != null)
            {
                src = new ProgressCodeWriter(src, status);
                res = new ProgressCodeWriter(res, status);
            }
            build(src, res);
        }

        /// <summary>
        /// A convenience method for <code>build(destDir,System.out)</code>.
        /// </summary>
        
        public void build(DirectoryInfo destDir)
		{
            build(destDir, destDir);
		}

        /// <summary>
        /// A convenience method for <code>build(srcDir,resourceDir,System.out)</code>.
        /// </summary>
       
        public void build(DirectoryInfo srcDir, DirectoryInfo resourceDir)
		{
			build(srcDir,resourceDir, null);
		}

        /// <summary>
        /// A convenience method for <code>build(out,out)</code>.
        /// </summary>
        public void build(CodeWriter @out)
        {
            build(@out, @out);
        }

        /// <summary>
        /// Generates Java source code.
        /// </summary>      
        public void build(CodeWriter source, CodeWriter resource)
        {
            JPackage[] pkgs = _packages.Values.ToArray();
            // avoid concurrent modification exception
            foreach (JPackage pkg in pkgs)
            {
                pkg.build(source, resource);
            }
            source.close();
            resource.close();
        }

        /// <summary>
        /// Returns the number of files to be generated if
        /// <seealso cref="#build"/> is invoked now.
        /// </summary>
        public int countArtifacts()
        {
            int r = 0;
            JPackage[] pkgs = this._packages.Values.ToArray();
            // avoid concurrent modification exception
            foreach (JPackage pkg in pkgs)
            {
                r += pkg.countArtifacts();
            }
            return r;
        }


        /// <summary>
        /// Obtains a reference to an existing class from its Class object.
        /// 
        /// <para>
        /// The parameter may not be primitive.
        /// 
        /// </para>
        /// </summary>
        /// <seealso cref= #_ref(Class) for the version that handles more cases. </seealso>
        public JClass @ref(Type clazz)
        {
            JReferencedClass jrc = null;
            if (refClasses != null && refClasses.ContainsKey(clazz)) {
                jrc = (JReferencedClass)refClasses[clazz];
            }           
            if (jrc == null)
            {
                //comment
                //if (clazz.IsPrimitive && clazz.Name!="Boolean" && clazz.Name !="Byte")
                //{
                //    throw new System.ArgumentException(clazz + " is a primitive");
                //}
                if (clazz.IsArray)
                {
                    return new JArrayClass(this, _ref(clazz.GetElementType()));
                }
                else
                {
                    jrc = new JReferencedClass(this, clazz);
                    refClasses[clazz] = jrc;
                }
            }
            return jrc;
        }

        public JType _ref(Type c)
        {
            if (c.IsPrimitive || c.Name.Equals("Void"))
            {              
                return JType.parse(this, c.Name);
            }
            else
            {
                return @ref(c);
            }
        }

        /// <summary>
        /// Obtains a reference to an existing class from its fully-qualified
        /// class name.
        /// 
        /// <para>
        /// First, this method attempts to load the class of the given name.
        /// If that fails, we assume that the class is derived straight from
        /// <seealso cref="Object"/>, and return a <seealso cref="JClass"/>.
        /// </para>
        /// </summary>
        public JClass @ref(string fullyQualifiedClassName)
        {
            try
            {
                // try the context class loader first
                return @ref(SecureLoader.contextClassLoader.GetType(fullyQualifiedClassName));
            }
            catch (Exception)
            {
                // fall through
            }
         
            try
            {
                return @ref(Type.GetType(fullyQualifiedClassName));
            }
            catch (Exception)
            {
                // fall through
            }

            // assume it's not visible to us.
            return new JDirectClass(this, fullyQualifiedClassName);
        }

        /// <summary>
        /// Cached for <seealso cref="#wildcard()"/>.
        /// </summary>
        private JClass _wildcard;

        /// <summary>
        /// Gets a <seealso cref="JClass"/> representation for "?",
        /// which is equivalent to "? extends Object".
        /// </summary>
        public JClass wildcard()
        {
            if (_wildcard == null)
            {
                _wildcard = @ref(typeof(object)).wildcard();
            }
            return _wildcard;
        }

        /// <summary>
        /// Obtains a type object from a type name.
        /// 
        /// <para>
        /// This method handles primitive types, arrays, and existing <seealso cref="Class"/>es.
        /// 
        /// </para>
        /// </summary>       
        public JType parseType(string name)
        {
            // array
            if (name.EndsWith("[]", StringComparison.Ordinal))
            {
                return parseType(name.Substring(0, name.Length - 2)).array();
            }

            // try primitive type
            try
            {
                return JType.parse(this, name);
            }
            catch (System.ArgumentException)
            {
                ;
            }

            // existing class
            return (new TypeNameParser(this, name)).parseTypeName();
        }

        private sealed class TypeNameParser
        {
            private readonly JCodeModel outerInstance;

            internal readonly string s;
            internal int idx;

            public TypeNameParser(JCodeModel outerInstance, string s)
            {
                this.outerInstance = outerInstance;
                this.s = s;
            }

            /// <summary>
            /// Parses a type name token T (which can be potentially of the form Tr&ly;T1,T2,...>,
            /// or "? extends/super T".)
            /// </summary>
            /// <returns> the index of the character next to T. </returns>           
            internal JClass parseTypeName()
            {
                int start = idx;

                if (s[idx] == '?')
                {
                    // wildcard
                    idx++;
                    ws();
                    string head = s.Substring(idx);
                    if (head.StartsWith("extends", StringComparison.Ordinal))
                    {
                        idx += 7;
                        ws();
                        return parseTypeName().wildcard();
                    }
                    else
                    {
                        if (head.StartsWith("super", StringComparison.Ordinal))
                        {
                            throw new System.NotSupportedException("? super T not implemented");
                        }
                        else
                        {
                            // not supported
                            throw new System.ArgumentException("only extends/super can follow ?, but found " + s.Substring(idx));
                        }
                    }
                }

                while (idx < s.Length)
                {
                    char ch = s[idx];
                     CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
                     if (provider.IsValidIdentifier(Convert.ToString(ch)) || ch == '.')
                     {
                         idx++;
                     }
                     else
                     {
                         break;
                     }
                }

                JClass clazz = outerInstance.@ref(s.Substring(start, idx - start));

                return parseSuffix(clazz);
            }

            /// <summary>
            /// Parses additional left-associative suffixes, like type arguments
            /// and array specifiers.
            /// </summary>           
            internal JClass parseSuffix(JClass clazz)
            {
                if (idx == s.Length)
                {
                    return clazz; // hit EOL
                }

                char ch = s[idx];

                if (ch == '<')
                {
                    return parseSuffix(parseArguments(clazz));
                }

                if (ch == '[')
                {
                    if (s[idx + 1] == ']')
                    {
                        idx += 2;
                        return parseSuffix(clazz.array());
                    }
                    throw new System.ArgumentException("Expected ']' but found " + s.Substring(idx + 1));
                }

                return clazz;
            }

            /// <summary>
            /// Skips whitespaces
            /// </summary>
            internal void ws()
            {
                while (char.IsWhiteSpace(s[idx]) && idx < s.Length)
                {
                    idx++;
                }
            }

            /// <summary>
            /// Parses '&lt;T1,T2,...,Tn>'
            /// </summary>
            /// <returns> the index of the character next to '>' </returns>         
            internal JClass parseArguments(JClass rawType)
            {
                if (s[idx] != '<')
                {
                    throw new System.ArgumentException();
                }
                idx++;

                IList<JClass> args = new List<JClass>();

                while (true)
                {
                    args.Add(parseTypeName());
                    if (idx == s.Length)
                    {
                        throw new System.ArgumentException("Missing '>' in " + s);
                    }
                    char ch = s[idx];
                    if (ch == '>')
                    {
                        return rawType.narrow(args.ToArray());
                    }

                    if (ch != ',')
                    {
                        throw new System.ArgumentException(s);
                    }
                    idx++;
                }

            }
        }

        /// <summary>
        /// References to existing classes.
        /// 
        /// <para>
        /// JReferencedClass is kept in a pool so that they are shared.
        /// There is one pool for each JCodeModel object.
        /// 
        /// </para>
        /// <para>
        /// It is impossible to cache JReferencedClass globally only because
        /// there is the _package() method, which obtains the owner JPackage
        /// object, which is scoped to JCodeModel.
        /// </para>
        /// </summary>
        private class JReferencedClass : JClass, JDeclaration
        {
            private readonly JCodeModel outerInstance;

            internal readonly Type _class;

            internal JReferencedClass(JCodeModel outerInstance, Type _clazz)
                : base(outerInstance)
            {
                this.outerInstance = outerInstance;
                this._class = _clazz;
                Debug.Assert(!_class.IsArray);
            }

            public override string name()
            {
                return _class.Name.Replace('$', '.');
            }

            public override string fullName()
            {               
                return _class.FullName.Replace('$', '.');
            }

            public string binaryName()
            {               
                return _class.FullName;
            }

            public override JClass outer()
            {
                Type p = _class.DeclaringType;
                if (p == null)
                {
                    return null;
                }
                return outerInstance.@ref(p);
            }

            public override JPackage _package()
            {
                string name = fullName();

                // this type is array
                if (name.IndexOf('[') != -1)
                {
                    return outerInstance._package("");
                }

                // other normal case
                int idx = name.LastIndexOf('.');
                if (idx < 0)
                {
                    return outerInstance._package("");
                }
                else
                {
                    return outerInstance._package(name.Substring(0, idx));
                }
            }

            public override JClass _extends()
            {
                Type sp = _class.BaseType;
                if (sp == null)
                {
                    if (isInterface)
                    {
                        return owner().@ref(typeof(object));
                    }
                    return null;
                }
                else
                {
                    return outerInstance.@ref(sp);
                }
            }

            public override IEnumerator<JClass> _implements()
            {
                Type[] interfaces = _class.GetInterfaces();
                return new IteratorAnonymousInnerClass(this, interfaces);
            }

            private class IteratorAnonymousInnerClass : IEnumerator<JClass>
            {
                private readonly JReferencedClass outerInstance;

                private Type[] interfaces;

                public IteratorAnonymousInnerClass(JReferencedClass outerInstance, Type[] interfaces)
                {
                    this.outerInstance = outerInstance;
                    this.interfaces = interfaces;
                    idx = 0;
                }

                private int idx;
                public virtual bool hasNext()
                {
                    return idx < interfaces.Length;
                }
                public virtual JClass next()
                {
                    return outerInstance.outerInstance.@ref(interfaces[idx++]);
                }
                public virtual void remove()
                {
                    throw new System.NotSupportedException();
                }
                public void Dispose() { }
                public void Reset() { }
                public bool MoveNext()
                {
                    return this.MoveNext();
                }
                object System.Collections.IEnumerator.Current
                {
                    get { return ((System.Collections.IEnumerator)this.outerInstance).Current; }
                }

                JClass IEnumerator<JClass>.Current
                {
                    get { return ((IEnumerator<JClass>)this.outerInstance).Current; }
                }
            }

            public override bool isInterface
            {
                get
                {
                    return _class.IsInterface;
                }
            }

            public override bool isAbstract
            {
                get
                {
                    return _class.IsAbstract;
                }
            }

            public override JPrimitiveType PrimitiveType
            {
                get
                {
                    Type v = boxToPrimitive[_class];
                    if (v != null)
                    {                      
                        return JType.parse(outerInstance, v.FullName);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public bool isArray
            {
                get
                {
                    return false;
                }
            }

            public virtual void declare(JFormatter f)
            {
            }

            public override JTypeVar[] typeParams()
            {
                // TODO: does JDK 1.5 reflection provides these information?
                return base.typeParams();
            }

            public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
            {
                // TODO: does JDK 1.5 reflection provides these information?
                return this;
            }
        }

        /// <summary>
        /// Conversion from primitive type <seealso cref="Class"/> (such as <seealso cref="Integer#TYPE"/>
        /// to its boxed type (such as <tt>Integer.class</tt>)
        /// </summary>
        public static readonly IDictionary<Type, Type> primitiveToBox;
        /// <summary>
        /// The reverse look up for <seealso cref="#primitiveToBox"/>
        /// </summary>
        public static readonly IDictionary<Type, Type> boxToPrimitive;

        static JCodeModel()
        {
            
            IDictionary<Type, Type> m1 = new Dictionary<Type, Type>();
            IDictionary<Type, Type> m2 = new Dictionary<Type, Type>();

            m1[typeof(bool)] = typeof(bool);
            m1[typeof(byte)] = typeof(byte);
            m1[typeof(char)] = typeof(char);
            m1[typeof(double)] = typeof(double);
            m1[typeof(float)] = typeof(float);
            m1[typeof(int)] = typeof(int) ;
            m1[typeof(long)] = typeof(long);
            m1[typeof(short)] = typeof(short);
            m1[typeof(void)] = typeof(void);

            foreach (KeyValuePair<Type, Type> e in m1)
            {
                m2[e.Value] = e.Key;
            }

            boxToPrimitive = m1;
            primitiveToBox = m2;
        }
    }
}
