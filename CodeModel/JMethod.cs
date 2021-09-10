//-----------------------------------------------------------------------
// <copyright file="JMethod.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeModel.util;

namespace CodeModel
{
    public class JMethod : JGenerifiableImpl, JDeclaration, JAnnotatable, JDocCommentable
    {

        /// <summary>
        /// Modifiers for this method
        /// </summary>      
        private JMods _mods;

        /// <summary>
        /// Return type for this method
        /// </summary>      
        private JType _type = null;

        /// <summary>
        /// Name of this method
        /// </summary>      
        private string _name = null;

        /// <summary>
        /// List of parameters for this method's declaration
        /// </summary>        
        private readonly IList<JVar> _params = new List<JVar>();

        /// <summary>
        /// Set of exceptions that this method may throw.
        /// A set instance lazily created.
        /// </summary>
       
        private ISet<JClass> @throws;

        /// <summary>
        /// JBlock of statements that makes up the body this method
        /// </summary>    
        private JBlock _body = null;

        private JDefinedClass outer;

        /// <summary>
        /// javadoc comments for this JMethod
        /// </summary>
        private JDocComment jdoc = null;

        /// <summary>
        /// Variable parameter for this method's varargs declaration
        /// introduced in J2SE 1.5
        /// </summary>     
        private JVar _varParam = null;

        /// <summary>
        /// Annotations on this variable. Lazily created.
        /// </summary>  
        private IList<JAnnotationUse> _annotations = null;


        private bool isConstructor
        {
            get
            {
                return _type == null;
            }
        }

        /// <summary>
        /// To set the default value for the
        ///  annotation member
        /// </summary>
        private JExpression defaultValue = null;


        /// <summary>
        /// JMethod constructor
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this method's declaration
        /// </param>
        /// <param name="type">
        ///        Return type for the method
        /// </param>
        /// <param name="name">
        ///        Name of this method </param>
        internal JMethod(JDefinedClass outer, int mods, JType type, string name)
        {
            this._mods = JMods.forMethod(mods);
            this._type = type;
            this._name = name;
            this.outer = outer;
        }

        /// <summary>
        /// Constructor constructor
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this constructor's declaration
        /// </param>
        /// <param name="_class">
        ///        JClass containing this constructor </param>
        internal JMethod(int mods, JDefinedClass _class)
        {
            this._mods = JMods.forMethod(mods);
            this._type = null;
            this._name = _class.name();
            this.outer = _class;
        }

        private ISet<JClass> Throws
        {
            get
            {
                if (@throws == null)
                {
                    @throws = new SortedSet<JClass>(ClassNameComparator.theInstance);
                }
                return @throws;
            }
        }

        /// <summary>
        /// Add an exception to the list of exceptions that this
        /// method may throw.
        /// </summary>
        /// <param name="exception">
        ///        Name of an exception that this method may throw </param>
        public virtual JMethod _throws(JClass exception)
        {
            Throws.Add(exception);
            return this;
        }

        public virtual JMethod _throws(Type exception)
        {
            return _throws(outer.owner().@ref(exception));
        }

        /// <summary>
        /// Returns the list of variable of this method.
        /// </summary>
        /// <returns> List of parameters of this method. This list is not modifiable. </returns>
        public virtual IList<JVar> @params()
        {
            return _params;
        }

        /// <summary>
        /// Add the specified variable to the list of parameters
        /// for this method signature.
        /// </summary>
        /// <param name="type">
        ///        JType of the parameter being added
        /// </param>
        /// <param name="name">
        ///        Name of the parameter being added
        /// </param>
        /// <returns> New parameter variable </returns>
        public virtual JVar param(int mods, JType type, string name)
        {
            JVar v = new JVar(JMods.forVar(mods), type, name, null);
            _params.Add(v);
            return v;
        }

        public virtual JVar param(JType type, string name)
        {
            return param(JMod.NONE, type, name);
        }

        public virtual JVar param(int mods, Type type, string name)
        {
            return param(mods, outer.owner()._ref(type), name);
        }

        public virtual JVar param(Type type, string name)
        {
            return param(outer.owner()._ref(type), name);
        }

        /// <seealso cref= #varParam(JType, String) </seealso>
        public virtual JVar varParam(Type type, string name)
        {
            return varParam(outer.owner()._ref(type), name);
        }

        /// <summary>
        /// Add the specified variable argument to the list of parameters
        /// for this method signature.
        /// </summary>
        /// <param name="type">
        ///      Type of the parameter being added.
        /// </param>
        /// <param name="name">
        ///        Name of the parameter being added
        /// </param>
        /// <returns> the variable parameter
        /// </returns>
        /// <exception cref="IllegalStateException">
        ///      If this method is called twice.
        ///      varargs in J2SE 1.5 can appear only once in the 
        ///      method signature. </exception>
        public virtual JVar varParam(JType type, string name)
        {
            if (!hasVarArgs())
            {

                _varParam = new JVar(JMods.forVar(JMod.NONE), type.array(), name, null);
                return _varParam;
            }
            else
            {
                throw new System.InvalidOperationException("Cannot have two varargs in a method,\n" + "Check if varParam method of JMethod is" + " invoked more than once");

            }

        }

        /// <summary>
        /// Adds an annotation to this variable. </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the field with </param>
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

        /// <summary>
        /// Adds an annotation to this variable.
        /// </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the field with </param>
        public virtual JAnnotationUse annotate(Type clazz)
        {
            return annotate(owner().@ref(clazz));
        }

        public virtual W annotate2<W, A>(W clazz) where W : JAnnotationWriter<A> where A: Attribute
        {
            return TypedAnnotationWriter<A, W>.create<A, W>(clazz, this);
        }

        public virtual ICollection<JAnnotationUse> annotations()
        {
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            return _annotations;
        }

        /// <summary>
        /// Check if there are any varargs declared
        /// for this method signature.
        /// </summary>
        public virtual bool hasVarArgs()
        {
            return this._varParam != null;
        }

        public virtual string name()
        {
            return _name;
        }

        /// <summary>
        /// Changes the name of the method.
        /// </summary>
        public virtual void name(string n)
        {
            this._name = n;
        }

        /// <summary>
        /// Returns the return type.
        /// </summary>
        public virtual JType type()
        {
            return _type;
        }

        /// <summary>
        /// Overrides the return type.
        /// </summary>
        public virtual void type(JType t)
        {
            this._type = t;
        }

        /// <summary>
        /// Returns all the parameter types in an array.
        /// @return
        ///      If there's no parameter, an empty array will be returned.
        /// </summary>
        public virtual JType[] listParamTypes()
        {
            JType[] r = new JType[_params.Count];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = _params[i].type();
            }
            return r;
        }

        /// <summary>
        /// Returns  the varags parameter type.
        /// @return
        /// If there's no vararg parameter type, null will be returned.
        /// </summary>
        public virtual JType listVarParamType()
        {
            if (_varParam != null)
            {
                return _varParam.type();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all the parameters in an array.
        /// @return
        ///      If there's no parameter, an empty array will be returned.
        /// </summary>
        public virtual JVar[] listParams()
        {
            return _params.ToArray();
        }

        /// <summary>
        /// Returns the variable parameter
        /// @return
        ///      If there's no parameter, null will be returned.
        /// </summary>
        public virtual JVar listVarParam()
        {
            return _varParam;
        }

        /// <summary>
        /// Returns true if the method has the specified signature.
        /// </summary>
        public virtual bool hasSignature(JType[] argTypes)
        {
            JVar[] p = listParams();
            if (p.Length != argTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < p.Length; i++)
            {
                if (!p[i].type().Equals(argTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the block that makes up body of this method
        /// </summary>
        /// <returns> Body of method </returns>
        public virtual JBlock body()
        {
            if (_body == null)
            {
                _body = new JBlock();
            }
            return _body;
        }

        /// <summary>
        /// Specify the default value for this annotation member </summary>
        /// <param name="value"> 
        ///           Default value for the annotation member
        ///  </param>
        public virtual void declareDefaultValue(JExpression value)
        {
            this.defaultValue = value;
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

        public override void declare(JFormatter f)
        {
            if (jdoc != null)
            {
                f.g((JGenerable)jdoc);
            }

            if (_annotations != null)
            {
                foreach (JAnnotationUse a in _annotations)
                {
                    f.g(a).nl();
                }
            }

            f.g(_mods);

            // declare the generics parameters
            base.declare(f);

            if (!isConstructor)
            {
                f.g(_type);
            }
            f.id(_name).p('(').i();
            // when parameters are printed in new lines, we want them to be indented.
            // there's a good chance no newlines happen, too, but just in case it does.
            bool first = true;
            foreach (JVar var in _params)
            {
                if (!first)
                {
                    f.p(',');
                }
                if (var.isAnnotated)
                {
                    f.nl();
                }
                f.b(var);
                first = false;
            }
            if (hasVarArgs())
            {
                if (!first)
                {
                    f.p(',');
                }
                f.g(_varParam.type().elementType());
                f.p("... ");
                f.id(_varParam.name());
            }

            f.o().p(')');
            if (@throws != null && @throws.Count > 0)
            {
                f.nl().i().p("throws").g(@throws).nl().o();
            }

            if (defaultValue != null)
            {
                f.p("default ");
                f.g(defaultValue);
            }
            if (_body != null)
            {
                f.s(_body);
            }
            else if (!outer.isInterface && !outer.isAnnotationTypeDeclaration && !_mods.isAbstract && !_mods.isNative)
            {
                // Print an empty body for non-native, non-abstract methods
                f.s(new JBlock());
            }
            else
            {
                f.p(';').nl();
            }
        }

        /// <summary>
        /// @return
        ///      the current modifiers of this method.
        ///      Always return non-null valid object.
        /// </summary>
        public virtual JMods mods()
        {
            return _mods;
        }

        /// @deprecated use <seealso cref="#mods()"/>  
        public virtual JMods Mods
        {
            get
            {
                return _mods;
            }
        }

        protected internal override JCodeModel owner()
        {
            return outer.owner();
        }
    }
}
