//-----------------------------------------------------------------------
// <copyright file="JClass.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    /// <summary>
    /// Represents a Java reference type, such as a class, an interface,
    /// an enum, an array type, a parameterized type.
    /// 
    /// <para>
    /// To be exact, this object represents an "use" of a reference type,
    /// not necessarily a declaration of it, which is modeled as <seealso cref="JDefinedClass"/>.
    /// </para>
    /// </summary>
    public abstract class JClass : JType
    {
        protected internal JClass(JCodeModel _owner)
        {
            this._owner = _owner;
        }

        /// <summary>
        /// Gets the package to which this class belongs.
        /// TODO: shall we move move this down?
        /// </summary>
        public abstract JPackage _package();

        /// <summary>
        /// Returns the class in which this class is nested, or <tt>null</tt> if
        /// this is a top-level class.
        /// </summary>
        public virtual JClass outer()
        {
            return null;
        }

        private readonly JCodeModel _owner;
        /// <summary>
        /// Gets the JCodeModel object to which this object belongs. </summary>
        public override JCodeModel owner()
        {
            return _owner;
        }

        /// <summary>
        /// Gets the super class of this class.
        /// 
        /// @return
        ///      Returns the JClass representing the superclass of the
        ///      entity (class or interface) represented by this <seealso cref="JClass"/>.
        ///      Even if no super class is given explicitly or this <seealso cref="JClass"/>
        ///      is not a class, this method still returns
        ///      <seealso cref="JClass"/> for <seealso cref="Object"/>.
        ///      If this JClass represents <seealso cref="Object"/>, return null.
        /// </summary>
        public abstract JClass _extends();

        /// <summary>
        /// Iterates all super interfaces directly implemented by
        /// this class/interface.
        /// 
        /// @return
        ///		A non-null valid iterator that iterates all
        ///		<seealso cref="JClass"/> objects that represents those interfaces
        ///		implemented by this object.
        /// </summary>
        public abstract IEnumerator<JClass> _implements();

        /// <summary>
        /// Iterates all the type parameters of this class/interface.
        /// 
        /// <para>
        /// For example, if this <seealso cref="JClass"/> represents 
        /// <code>Set&lt;T></code>, this method returns an array
        /// that contains single <seealso cref="JTypeVar"/> for 'T'.
        /// </para>
        /// </summary>
        public virtual JTypeVar[] typeParams()
        {
            return EMPTY_ARRAY;
        }

        /// <summary>
        /// Sometimes useful reusable empty array.
        /// </summary>
        protected internal static readonly JTypeVar[] EMPTY_ARRAY = new JTypeVar[0];

        /// <summary>
        /// Checks if this object represents an interface.
        /// </summary>
        public abstract bool isInterface { get; }

        /// <summary>
        /// Checks if this class is an abstract class.
        /// </summary>
        public abstract bool isAbstract { get; }

        /// <summary>
        /// If this class represents one of the wrapper classes
        /// defined in the java.lang package, return the corresponding
        /// primitive type. Otherwise null.
        /// </summary>
        public virtual JPrimitiveType PrimitiveType
        {
            get
            {
                return null;
            }
        }

        /// @deprecated calling this method from <seealso cref="JClass"/>
        /// would be meaningless, since it's always guaranteed to
        /// return <tt>this</tt>. 
        public override JClass boxify()
        {
            return this;
        }

        public override JType unboxify()
        {
            JPrimitiveType pt = PrimitiveType;
            return pt == null ? (JType)this : pt;
        }

        public override JType erasure()
        {
            return this;
        }

        /// <summary>
        /// Checks the relationship between two classes.
        /// <para>
        /// This method works in the same way as <seealso cref="Class#isAssignableFrom(Class)"/>
        /// works. For example, baseClass.isAssignableFrom(derivedClass)==true.
        /// </para>
        /// </summary>
        public bool isAssignableFrom(JClass derived)
        {
            // to avoid the confusion, always use "this" explicitly in this method.

            // null can be assigned to any type.
            if (derived is JNullType)
            {
                return true;
            }

            if (this == derived)
            {
                return true;
            }

            // the only class that is assignable from an interface is
            // java.lang.Object
            if (this == _package().owner().@ref(typeof(object)))
            {
                return true;
            }

            JClass b = derived._extends();
            if (b != null && this.isAssignableFrom(b))
            {
                return true;
            }

            if (this.isInterface)
            {
                IEnumerator<JClass> itfs = derived._implements();
                while (itfs.MoveNext())
                {
                    if (this.isAssignableFrom(itfs.Current))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the parameterization of the given base type.
        /// 
        /// <para>
        /// For example, given the following
        /// <pre><xmp>
        /// interface Foo<T> extends List<List<T>> {}
        /// interface Bar extends Foo<String> {}
        /// </xmp></pre>
        /// This method works like this:
        /// <pre><xmp>
        /// getBaseClass( Bar, List ) = List<List<String>
        /// getBaseClass( Bar, Foo  ) = Foo<String>
        /// getBaseClass( Foo<? extends Number>, Collection ) = Collection<List<? extends Number>>
        /// getBaseClass( ArrayList<? extends BigInteger>, List ) = List<? extends BigInteger>
        /// </xmp></pre>
        /// 
        /// </para>
        /// </summary>
        /// <param name="baseType">
        ///      The class whose parameterization we are interested in.
        /// @return
        ///      The use of {@code baseType} in {@code this} type.
        ///      or null if the type is not assignable to the base type. </param>
        public JClass getBaseClass(JClass baseType)
        {

            if (this.erasure().Equals(baseType))
            {
                return this;
            }

            JClass b = _extends();
            if (b != null)
            {
                JClass bc = b.getBaseClass(baseType);
                if (bc != null)
                {
                    return bc;
                }
            }

            IEnumerator<JClass> itfs = _implements();
            while (itfs.MoveNext())
            {
                JClass bc = itfs.Current.getBaseClass(baseType);
                if (bc != null)
                {
                    return bc;
                }
            }

            return null;
        }

        public JClass getBaseClass(Type baseType)
        {
            return getBaseClass(owner().@ref(baseType));
        }


        private JClass arrayClass;
        public override JClass array()
        {
            if (arrayClass == null)
            {
                arrayClass = new JArrayClass(owner(), this);
            }
            return arrayClass;
        }

        /// <summary>
        /// "Narrows" a generic class to a concrete class by specifying
        /// a type argument.
        /// 
        /// <para>
        /// <code>.narrow(X)</code> builds <code>Set&lt;X></code> from <code>Set</code>.
        /// </para>
        /// </summary>
        public virtual JClass narrow(Type clazz)
        {
            return narrow(owner().@ref(clazz));
        }

        public virtual JClass narrow(params Type[] clazz)
        {
            JClass[] r = new JClass[clazz.Length];
            for (int i = 0; i < clazz.Length; i++)
            {
                r[i] = owner().@ref(clazz[i]);
            }
            return narrow(r);
        }

        /// <summary>
        /// "Narrows" a generic class to a concrete class by specifying
        /// a type argument.
        /// 
        /// <para>
        /// <code>.narrow(X)</code> builds <code>Set&lt;X></code> from <code>Set</code>.
        /// </para>
        /// </summary>
        public virtual JClass narrow(JClass clazz)
        {
            return new JNarrowedClass(this, clazz);
        }

        public virtual JClass narrow(JType type)
        {
            return narrow(type.boxify());
        }

        public virtual JClass narrow(params JClass[] clazz)
        {
            return new JNarrowedClass(this, clazz.ToList());
        }

        public virtual JClass narrow<T1>(IList<T1> clazz) where T1 : JClass
        {
            return new JNarrowedClass(this, new List<JClass>(clazz));
        }

        /// <summary>
        /// If this class is parameterized, return the type parameter of the given index.
        /// </summary>
        public virtual IList<JClass> TypeParameters
        {
            get
            {
                return new List<JClass>();
            }
        }

        /// <summary>
        /// Returns true if this class is a parameterized class.
        /// </summary>
        public bool isParameterized
        {
            get
            {
                return erasure() != this;
            }
        }

        /// <summary>
        /// Create "? extends T" from T.
        /// </summary>
        /// <returns> never null </returns>
        public JClass wildcard()
        {
            return new JTypeWildcard(this);
        }

        /// <summary>
        /// Substitutes the type variables with their actual arguments.
        /// 
        /// <para>
        /// For example, when this class is Map&lt;String,Map&lt;V>>,
        /// (where V then doing
        /// substituteParams( V, Integer ) returns a <seealso cref="JClass"/>
        /// for <code>Map&lt;String,Map&lt;Integer>></code>.
        /// 
        /// </para>
        /// <para>
        /// This method needs to work recursively.
        /// </para>
        /// </summary>
        public abstract JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings);

        public override string ToString()
        {          
            return this.GetType().FullName + '(' + name() + ')';
        }


        public JExpression dotclass()
        {
            return JExpr.dotclass(this);
        }

        /// <summary>
        /// Generates a static method invocation. </summary>
        public JInvocation staticInvoke(JMethod method)
        {
            return new JInvocation(this, method);
        }

        /// <summary>
        /// Generates a static method invocation. </summary>
        public JInvocation staticInvoke(string method)
        {
            return new JInvocation(this, method);
        }

        /// <summary>
        /// Static field reference. </summary>
        public JFieldRef staticRef(string field)
        {
            return new JFieldRef(this, field);
        }

        /// <summary>
        /// Static field reference. </summary>
        public JFieldRef staticRef(JVar field)
        {
            return new JFieldRef(this, field);
        }

        public override void generate(JFormatter f)
        {
            f.t(this);
        }

        /// <summary>
        /// Prints the class name in javadoc @link format.
        /// </summary>
        internal virtual void printLink(JFormatter f)
        {
            f.p("{@link ").g(this).p('}');
        }
    }
}
