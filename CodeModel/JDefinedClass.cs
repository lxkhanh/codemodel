//-----------------------------------------------------------------------
// <copyright file="JDefinedClass.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace CodeModel
{ 
    public class JDefinedClass : JClass, JDeclaration, JClassContainer, JGenerifiable, JAnnotatable, JDocCommentable
    {

        /// <summary>
        /// Name of this class. Null if anonymous. </summary>
        
        private string _name = null;

        /// <summary>
        /// Modifiers for the class declaration </summary>
  
        private JMods _mods;

        /// <summary>
        /// Name of the super class of this class. </summary>
        private JClass superClass;

        /// <summary>
        /// List of interfaces that this class implements </summary>
        private readonly ISet<JClass> interfaces = new SortedSet<JClass>();

        /// <summary>
        /// Fields keyed by their names. </summary>
        /*package*/       
        internal readonly IDictionary<string, JFieldVar> _fields = new Dictionary<string, JFieldVar>();

        /// <summary>
        /// Static initializer, if this class has one </summary>
        private JBlock _init = null;

        /// <summary>
        /// Instance initializer, if this class has one </summary>
        private JBlock _instanceInit = null;

        /// <summary>
        /// class javadoc </summary>
        private JDocComment jdoc = null;

        /// <summary>
        /// Set of constructors for this class, if any </summary>
        private readonly IList<JMethod> _constructors = new List<JMethod>();

        /// <summary>
        /// Set of methods that are members of this class </summary>      
        private readonly IList<JMethod> _methods = new List<JMethod>();

        /// <summary>
        /// Nested classes as a map from name to JDefinedClass.
        /// The name is all capitalized in a case sensitive file system
        /// (<seealso cref="JCodeModel#isCaseSensitiveFileSystem"/>) to avoid conflicts.
        /// 
        /// Lazily created to save footprint.
        /// </summary>
        /// <seealso cref= #getClasses() </seealso>
        private IDictionary<string, JDefinedClass> _classes;


        /// <summary>
        /// Flag that controls whether this class should be really generated or not.
        /// 
        /// Sometimes it is useful to generate code that refers to class X,
        /// without actually generating the code of X.
        /// This flag is used to surpress X.java file in the output.
        /// </summary>
        private bool hideFile = false;

        /// <summary>
        /// Client-app spcific metadata associated with this user-created class.
        /// </summary>
        public object metadata;

        /// <summary>
        /// String that will be put directly inside the generated code.
        /// Can be null.
        /// </summary>
        private string directBlock;

        /// <summary>
        /// If this is a package-member class, this is <seealso cref="JPackage"/>.
        /// If this is a nested class, this is <seealso cref="JDefinedClass"/>.
        /// If this is an anonymous class, this constructor shouldn't be used.
        /// </summary>       
        private JClassContainer _outer = null;


        /// <summary>
        /// Default value is class or interface
        ///  or annotationTypeDeclaration
        ///  or enum
        /// 
        /// </summary>
        private readonly ClassType classType;

        /// <summary>
        /// List containing the enum value declarations
        /// 
        /// </summary>
        //    private List enumValues = new ArrayList();

        /// <summary>
        /// Set of enum constants that are keyed by names.
        /// In Java, enum constant order is actually significant,
        /// because of order ID they get. So let's preserve the order.
        /// </summary>
        private readonly IDictionary<string, JEnumConstant> enumConstantsByName = new Dictionary<string, JEnumConstant>();

        /// <summary>
        /// Annotations on this variable. Lazily created.
        /// </summary>      
        private IList<JAnnotationUse> _annotations = null;


        /// <summary>
        /// Helper class to implement <seealso cref="JGenerifiable"/>.
        /// </summary>
        private readonly JGenerifiableImpl generifiable = new JGenerifiableImplAnonymousInnerClass();

        private class JGenerifiableImplAnonymousInnerClass : JGenerifiableImpl
        {
            JDefinedClass outerInstance;
            public JGenerifiableImplAnonymousInnerClass()
            {
            }

            protected internal override JCodeModel owner()
            {
                return outerInstance.owner();
            }
        }

        internal JDefinedClass(JClassContainer parent, int mods, string name, ClassType classTypeval)
            : this(mods, name, parent, parent.owner(), classTypeval)
        {
        }

        /// <summary>
        /// Constructor for creating anonymous inner class.
        /// </summary>
        internal JDefinedClass(JCodeModel owner, int mods, string name)
            : this(mods, name, null, owner)
        {
        }

        private JDefinedClass(int mods, string name, JClassContainer parent, JCodeModel owner)
            : this(mods, name, parent, owner, ClassType.CLASS)
        {
        }

        /// <summary>
        /// JClass constructor
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this class declaration
        /// </param>
        /// <param name="name">
        ///        Name of this class </param>
        private JDefinedClass(int mods, string name, JClassContainer parent, JCodeModel owner, ClassType classTypeVal)
            : base(owner)
        {

            if (!string.ReferenceEquals(name, null))
            {
                if (name.Trim().Length == 0)
                {
                    throw new System.ArgumentException("JClass name empty");
                }

                // using System.CodeDom.Compiler;
                CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
                if (!provider.IsValidIdentifier(char.ToString(name[0])))
                {
                     string msg = "JClass name "
                            + name + " contains illegal character"
                            + " for beginning of identifier: "
                            + name[0];
                    throw new System.ArgumentException(msg);
                }

                if (!provider.IsValidIdentifier(name))
                {
                    string msg = "JClass name "
                                + name + " contains illegal character "
                                + name;
                    throw new System.ArgumentException(msg);
                }
                //for (int i = 1; i < name.Length; i++)
                //{
                //    if (!provider.i(char.ToString(name[i])))
                //    {
                //        string msg = "JClass name "
                //                + name + " contains illegal character "
                //                + name[i];
                //        throw new System.ArgumentException(msg);
                //    }
                //}
            }

            this.classType = classTypeVal;
            if (isInterface)
            {
                this._mods = JMods.forInterface(mods);
            }
            else
            {
                this._mods = JMods.forClass(mods);
            }

            this._name = name;

            this._outer = parent;
        }

        /// <summary>
        /// Returns true if this is an anonymous class.
        /// </summary>
        public bool isAnonymous
        {
            get
            {
                return string.ReferenceEquals(_name, null);
            }
        }

        /// <summary>
        /// This class extends the specifed class.
        /// </summary>
        /// <param name="superClass">
        ///        Superclass for this class
        /// </param>
        /// <returns> This class </returns>
        public JDefinedClass _extends(JClass superClass)
        {
            if (this.classType == ClassType.INTERFACE)
            {
                if (superClass.isInterface)
                {
                    return this._implements(superClass);
                }
                else
                {
                    throw new System.ArgumentException("unable to set the super class for an interface");
                }
            }
            if (superClass == null)
            {
                throw new System.NullReferenceException();
            }

            for (JClass o = superClass.outer(); o != null; o = o.outer())
            {
                if (this == o)
                {
                    throw new System.ArgumentException("Illegal class inheritance loop." + "  Outer class " + this._name + " may not subclass from inner class: " + o.name());
                }
            }

            this.superClass = superClass;
            return this;
        }

        public virtual JDefinedClass _extends(Type superClass)
        {
            return _extends(owner().@ref(superClass));
        }

        /// <summary>
        /// Returns the class extended by this class.
        /// </summary>
        public override JClass _extends()
        {
            if (superClass == null)
            {
                superClass = owner().@ref(typeof(object));
            }
            return superClass;
        }

        /// <summary>
        /// This class implements the specifed interface.
        /// </summary>
        /// <param name="iface">
        ///        Interface that this class implements
        /// </param>
        /// <returns> This class </returns>
        public virtual JDefinedClass _implements(JClass iface)
        {
            interfaces.Add(iface);
            return this;
        }

        public virtual JDefinedClass _implements(Type iface)
        {
            return _implements(owner().@ref(iface));
        }

        /// <summary>
        /// Returns an iterator that walks the nested classes defined in this
        /// class.
        /// </summary>
        public override IEnumerator<JClass> _implements()
        {
            return interfaces.GetEnumerator();
        }

        /// <summary>
        /// JClass name accessor.
        /// 
        /// <para>
        /// For example, for <code>java.util.List</code>, this method
        /// returns <code>"List"</code>"
        /// 
        /// </para>
        /// </summary>
        /// <returns> Name of this class </returns>
        public override string name()
        {
            return _name;
        }

        /// <summary>
        /// If the named enum already exists, the reference to it is returned.
        /// Otherwise this method generates a new enum reference with the given
        /// name and returns it.
        /// </summary>
        /// <param name="name">
        ///  	The name of the constant.
        /// @return
        ///      The generated type-safe enum constant. </param>
        public virtual JEnumConstant enumConstant(string name)
        {
            JEnumConstant ec = null;
            if (enumConstantsByName != null && enumConstantsByName.ContainsKey(name)) {
                ec = enumConstantsByName[name];
            }         
            if (null == ec)
            {
                ec = new JEnumConstant(this, name);
                enumConstantsByName[name] = ec;
            }
            return ec;
        }

        /// <summary>
        /// Gets the fully qualified name of this class.
        /// </summary>
        public override string fullName()
        {
            if (_outer is JDefinedClass)
            {
                return ((JDefinedClass)_outer).fullName() + '.' + name();
            }

            JPackage p = _package();
            if (p.isUnnamed)
            {
                return name();
            }
            else
            {
                return p.name + '.' + name();
            }
        }

        public string binaryName()
        {
            if (_outer is JDefinedClass)
            {
                return ((JDefinedClass)_outer).binaryName() + '$' + name();
            }
            else
            {
                return fullName();
            }
        }

        public override bool isInterface
        {
            get
            {
                return this.classType == ClassType.INTERFACE;
            }
        }

        public override bool isAbstract
        {
            get
            {
                return _mods.isAbstract;
            }
        }

        /// <summary>
        /// Adds a field to the list of field members of this JDefinedClass.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this field
        /// </param>
        /// <param name="type">
        ///        JType of this field
        /// </param>
        /// <param name="name">
        ///        Name of this field
        /// </param>
        /// <returns> Newly generated field </returns>
        public virtual JFieldVar field(int mods, JType type, string name)
        {
            return field(mods, type, name, null);
        }

        public virtual JFieldVar field(int mods, Type type, string name)
        {
            return field(mods, owner()._ref(type), name);
        }

        /// <summary>
        /// Adds a field to the list of field members of this JDefinedClass.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this field. </param>
        /// <param name="type">
        ///        JType of this field. </param>
        /// <param name="name">
        ///        Name of this field. </param>
        /// <param name="init">
        ///        Initial value of this field.
        /// </param>
        /// <returns> Newly generated field </returns>
        public virtual JFieldVar field(int mods, JType type, string name, JExpression init)
        {
            JFieldVar f = new JFieldVar(this, JMods.forField(mods), type, name, init);

            if (_fields!=null && _fields.ContainsKey(name))
            {
                throw new System.ArgumentException("trying to create the same field twice: " + name);
            }

            _fields.Add(name, f);
            return f;
        }

        /// <summary>
        ///  This method indicates if the interface
        ///   is an annotationTypeDeclaration
        /// 
        /// </summary>
        public virtual bool isAnnotationTypeDeclaration
        {
            get
            {
                return this.classType == ClassType.ANNOTATION_TYPE_DECL;


            }
        }

        /// <summary>
        /// Add an annotationType Declaration to this package </summary>
        /// <param name="name">
        ///      Name of the annotation Type declaration to be added to this package
        /// @return
        ///      newly created Annotation Type Declaration </param>
        
        public virtual JDefinedClass _annotationTypeDeclaration(string name)
        {
            return _class(JMod.PUBLIC, name, ClassType.ANNOTATION_TYPE_DECL);
        }

        /// <summary>
        /// Add a public enum to this package </summary>
        /// <param name="name">
        ///      Name of the enum to be added to this package
        /// @return
        ///      newly created Enum </param>     
        
        public virtual JDefinedClass _enum(string name)
        {
            return _class(JMod.PUBLIC, name, ClassType.ENUM);
        }

        /// <summary>
        /// Add a public enum to this package </summary>
        /// <param name="name">
        ///      Name of the enum to be added to this package </param>
        /// <param name="mods">
        /// 		Modifiers for this enum declaration
        /// @return
        ///      newly created Enum </param>
        
        public virtual JDefinedClass _enum(int mods, string name)
        {
            return _class(mods, name, ClassType.ENUM);
        }

        public virtual ClassType ClassType
        {
            get
            {
                return this.classType;
            }
        }

        public virtual JFieldVar field(int mods, Type type, string name, JExpression init)
        {
            return field(mods, owner()._ref(type), name, init);
        }

        /// <summary>
        /// Returns all the fields declred in this class.
        /// The returned <seealso cref="Map"/> is a read-only live view.
        /// </summary>
        /// <returns> always non-null. </returns>
        public virtual IDictionary<string, JFieldVar> fields()
        {
            return _fields;
        }

        /// <summary>
        /// Removes a <seealso cref="JFieldVar"/> from this class.
        /// </summary>
        /// <exception cref="IllegalArgumentException">
        ///      if the given field is not a field on this class.  </exception>
        public virtual void removeField(JFieldVar field)
        {
            if (!_fields.Remove(field.name()))
            {
                throw new System.ArgumentException();
            }
        }

        /// <summary>
        /// Creates, if necessary, and returns the static initializer
        /// for this class.
        /// </summary>
        /// <returns> JBlock containing initialization statements for this class </returns>
        public virtual JBlock init()
        {
            if (_init == null)
            {
                _init = new JBlock();
            }
            return _init;
        }

        /// <summary>
        /// Creates, if necessary, and returns the instance initializer
        /// for this class.
        /// </summary>
        /// <returns> JBlock containing initialization statements for this class </returns>
        public virtual JBlock instanceInit()
        {
            if (_instanceInit == null)
            {
                _instanceInit = new JBlock();
            }
            return _instanceInit;
        }

        /// <summary>
        /// Adds a constructor to this class.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this constructor </param>
        public virtual JMethod constructor(int mods)
        {
            JMethod c = new JMethod(mods, this);
            _constructors.Add(c);
            return c;
        }

        /// <summary>
        /// Returns an iterator that walks the constructors defined in this class.
        /// </summary>
        public virtual IEnumerator<JMethod> constructors()
        {
            return _constructors.GetEnumerator();
        }

        /// <summary>
        /// Looks for a method that has the specified method signature
        /// and return it.
        /// 
        /// @return
        ///      null if not found.
        /// </summary>
        public virtual JMethod getConstructor(JType[] argTypes)
        {
            foreach (JMethod m in _constructors)
            {
                if (m.hasSignature(argTypes))
                {
                    return m;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a method to the list of method members of this JDefinedClass instance.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this method
        /// </param>
        /// <param name="type">
        ///        Return type for this method
        /// </param>
        /// <param name="name">
        ///        Name of the method
        /// </param>
        /// <returns> Newly generated JMethod </returns>
        public virtual JMethod method(int mods, JType type, string name)
        {
            // XXX problems caught in M constructor
            JMethod m = new JMethod(this, mods, type, name);
            _methods.Add(m);
            return m;
        }

        public virtual JMethod method(int mods, Type type, string name)
        {
            return method(mods, owner()._ref(type), name);
        }

        /// <summary>
        /// Returns the set of methods defined in this class.
        /// </summary>
        public virtual ICollection<JMethod> methods()
        {
            return _methods;
        }

        /// <summary>
        /// Looks for a method that has the specified method signature
        /// and return it.
        /// 
        /// @return
        ///      null if not found.
        /// </summary>
        public virtual JMethod getMethod(string name, JType[] argTypes)
        {
            foreach (JMethod m in _methods)
            {
                if (!m.name().Equals(name))
                {
                    continue;
                }

                if (m.hasSignature(argTypes))
                {
                    return m;
                }
            }
            return null;
        }

        public virtual bool isClass
        {
            get
            {
                return true;
            }
        }
        public virtual bool isPackage
        {
            get
            {
                return false;
            }
        }
        public JPackage package
        {
            get
            {
                return parentContainer().package;
            }
        }

        /// <summary>
        /// Add a new nested class to this class.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this class declaration
        /// </param>
        /// <param name="name">
        ///        Name of class to be added to this package
        /// </param>
        /// <returns> Newly generated class </returns>     
        public virtual JDefinedClass _class(int mods, string name)
        {
            return _class(mods, name, ClassType.CLASS);
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// @deprecated
        /// </summary>       
        public virtual JDefinedClass _class(int mods, string name, bool isInterface)
        {
            return _class(mods, name, isInterface ? ClassType.INTERFACE : ClassType.CLASS);
        }
      
        public virtual JDefinedClass _class(int mods, string name, ClassType classTypeVal)
        {
            string NAME;
            if (JCodeModel.isCaseSensitiveFileSystem)
            {
                NAME = name.ToUpper();
            }
            else
            {
                NAME = name;
            }

            if (Classes.ContainsKey(NAME))
            {
                throw new JClassAlreadyExistsException(Classes[NAME]);
            }
            else
            {
                // XXX problems caught in the NC constructor
                JDefinedClass c = new JDefinedClass(this, mods, name, classTypeVal);
                Classes.Add(NAME, c);
                return c;
            }
        }

        /// <summary>
        /// Add a new public nested class to this class.
        /// </summary>      
        public virtual JDefinedClass _class(string name)
        {
            return _class(JMod.PUBLIC, name);
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
       
        public virtual JDefinedClass _interface(int mods, string name)
        {
            return _class(mods, name, ClassType.INTERFACE);
        }

        /// <summary>
        /// Adds a public interface to this package.
        /// </summary>
       
        public virtual JDefinedClass _interface(string name)
        {
            return _interface(JMod.PUBLIC, name);
        }

        /// <summary>
        /// Creates, if necessary, and returns the class javadoc for this
        /// JDefinedClass
        /// </summary>
        /// <returns> JDocComment containing javadocs for this class </returns>
        public virtual JDocComment javadoc()
        {
            if (jdoc == null)
            {
                jdoc = new JDocComment(owner());
            }
            return jdoc;
        }

        /// <summary>
        /// Mark this file as hidden, so that this file won't be
        /// generated.
        /// 
        /// <para>
        /// This feature could be used to generate code that refers
        /// to class X, without actually generating X.java.
        /// </para>
        /// </summary>
        public virtual void hide()
        {
            hideFile = true;
        }

        public virtual bool isHidden
        {
            get
            {
                return hideFile;
            }
        }

        /// <summary>
        /// Returns an iterator that walks the nested classes defined in this
        /// class.
        /// </summary>
        public IEnumerator<JDefinedClass> classes()
        {
            if (_classes == null)
            {
                return System.Linq.Enumerable.Empty<JDefinedClass>().GetEnumerator();
            }
            else
            {
                if (_classes != null && _classes.Values != null) {
                    return _classes.Values.GetEnumerator();
                }
                return null;
            }
        }

        private IDictionary<string, JDefinedClass> Classes
        {
            get
            {
                if (_classes == null)
                {
                    _classes = new SortedDictionary<string, JDefinedClass>();
                }
                return _classes;
            }
        }


        /// <summary>
        /// Returns all the nested classes defined in this class.
        /// </summary>
        public JClass[] listClasses()
        {
            if (_classes == null)
            {
                return new JClass[0];
            }
            else
            {
                if (_classes != null && _classes.Values != null)
                {
                    return _classes.Values.ToArray();
                }
                return null;
            }
        }

        public override JClass outer()
        {
            if (_outer.isClass)
            {
                return (JClass)_outer;
            }
            else
            {
                return null;
            }
        }

        public virtual void declare(JFormatter f)
        {
            if (jdoc != null)
            {
                f.nl().g((JGenerable)jdoc);
            }

            if (_annotations != null)
            {
                foreach (JAnnotationUse annotation in _annotations)
                {
                    f.g(annotation).nl();
                }
            }

            f.g(_mods).p(classType.declarationToken).id(_name).d(generifiable);

            if (superClass != null && superClass != owner().@ref(typeof(object)))
            {
                f.nl().i().p("extends").g(superClass).nl().o();
            }

            if (interfaces.Count > 0)
            {
                if (superClass == null)
                {
                    f.nl();
                }
                f.i().p(classType == ClassType.INTERFACE ? "extends" : "implements");
                f.g(interfaces);
                f.nl().o();
            }
            declareBody(f);
        }

        /// <summary>
        /// prints the body of a class.
        /// </summary>
        protected internal virtual void declareBody(JFormatter f)
        {
            //f.p('{').nl().nl().i();
            f.p('{').nl().i();
            bool first = true;

            if (enumConstantsByName.Count > 0)
            {
                foreach (JEnumConstant c in enumConstantsByName.Values)
                {
                    if (!first)
                    {
                        f.p(',').nl();
                    }
                    f.d(c);
                    first = false;
                }
                f.p(';').nl();
            }

            foreach (JFieldVar field in _fields.Values)
            {
                f.d(field);
            }
            if (_init != null)
            {
                f.nl().p("static").s(_init);
            }
            if (_instanceInit != null)
            {
                f.nl().s(_instanceInit);
            }
            foreach (JMethod m in _constructors)
            {
                f.nl().d(m);
            }
            foreach (JMethod m in _methods)
            {
                f.nl().d(m);
            }
            if (_classes != null && _classes.Values != null)
            {
                foreach (JDefinedClass dc in _classes.Values)
                {
                    f.nl().d(dc);
                }
            }


            if (!string.ReferenceEquals(directBlock, null))
            {
                f.p(directBlock);
            }
            f.o().p('}').nl();
            //f.nl().o().p('}').nl();
        }

        /// <summary>
        /// Places the given string directly inside the generated class.
        /// 
        /// This method can be used to add methods/fields that are not
        /// generated by CodeModel.
        /// This method should be used only as the last resort.
        /// </summary>
        public virtual void direct(string @string)
        {
            if (string.ReferenceEquals(directBlock, null))
            {
                directBlock = @string;
            }
            else
            {
                directBlock += @string;
            }
        }

        public override JPackage _package()
        {
            JClassContainer p = _outer;
            while (!(p is JPackage))
            {
                p = p.parentContainer();
            }
            return (JPackage)p;
        }

        public JClassContainer parentContainer()
        {
            return _outer;
        }

        public virtual JTypeVar generify(string name)
        {
            return generifiable.generify(name);
        }
        public virtual JTypeVar generify(string name, Type bound)
        {
            return generifiable.generify(name, bound);
        }
        public virtual JTypeVar generify(string name, JClass bound)
        {
            return generifiable.generify(name, bound);
        }
        public override JTypeVar[] typeParams()
        {
            return generifiable.typeParams();
        }

        public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
        {
            return this;
        }

        /// <summary>
        /// Adding ability to annotate a class </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the class with </param>
        public virtual JAnnotationUse annotate(Type clazz)
        {
            return annotate(owner().@ref(clazz));
        }

        /// <summary>
        /// Adding ability to annotate a class </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the class with </param>
        public virtual JAnnotationUse annotate(JClass clazz)
        {
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            JAnnotationUse a = new JAnnotationUse(clazz);
            _annotations.Add(a);
            return a;
        }

        public virtual W annotate2<W, A>(W clazz) where W : JAnnotationWriter<A> where A: Attribute
        {
            return TypedAnnotationWriter<A, W>.create<A, W>(clazz, this);
        }

        /// <summary>
        /// <seealso cref="JAnnotatable#annotations()"/>
        /// </summary>
        public virtual ICollection<JAnnotationUse> annotations()
        {
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            return _annotations;
        }

        /// <summary>
        /// @return
        ///      the current modifiers of this class.
        ///      Always return non-null valid object.
        /// </summary>
        public virtual JMods mods()
        {
            return _mods;
        }
    }
}
