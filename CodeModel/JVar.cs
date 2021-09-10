//-----------------------------------------------------------------------
// <copyright file="JVar.cs">
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
    /// Variables and fields.
    /// </summary>

    public class JVar : JExpressionImpl, JDeclaration, JAssignmentTarget, JAnnotatable
    {

        /// <summary>
        /// Modifiers.
        /// </summary>
        private JMods _mods;

        /// <summary>
        /// JType of the variable
        /// </summary>
        private JType _type;

        /// <summary>
        /// Name of the variable
        /// </summary>
        private string _name;

        /// <summary>
        /// Initialization of the variable in its declaration
        /// </summary>
        private JExpression _init;

        /// <summary>
        /// Annotations on this variable. Lazily created.
        /// </summary>
        private IList<JAnnotationUse> _annotations = null;


        /// <summary>
        /// JVar constructor
        /// </summary>
        /// <param name="type">
        ///        Datatype of this variable
        /// </param>
        /// <param name="name">
        ///        Name of this variable
        /// </param>
        /// <param name="init">
        ///        Value to initialize this variable to </param>
        internal JVar(JMods mods, JType type, string name, JExpression init)
        {
            this._mods = mods;
            this._type = type;
            this._name = name;
            this._init = init;
        }


        /// <summary>
        /// Initialize this variable
        /// </summary>
        /// <param name="init">
        ///        JExpression to be used to initialize this field </param>
        public JVar init(JExpression init)
        {
            this._init = init;
            return this;
        }

        /// <summary>
        /// Get the name of this variable
        /// </summary>
        /// <returns> Name of the variable </returns>
        public string name()
        {
            return _name;           
        }

        /// <summary>
        /// Changes the name of this variable.
        /// </summary>
        public virtual void name(string name)
        {
            if (!JJavaName.isJavaIdentifier(name))
            {
                throw new System.ArgumentException();
            }
            this._name = name;
        }

        /// <summary>
        /// Return the type of this variable.
        /// @return
        ///      always non-null.
        /// </summary>
        public virtual JType type()
        {
            return _type; 
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

        /// <summary>
        /// Sets the type of this variable.
        /// </summary>
        /// <param name="newType">
        ///      must not be null.
        /// 
        /// @return
        ///      the old type value. always non-null. </param>
        public virtual JType type(JType newType)
        {
            JType r = _type;
            if (newType == null)
            {
                throw new System.ArgumentException();
            }
            _type = newType;
            return r;
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
            return annotate(_type.owner().@ref(clazz));
        }

        public virtual W annotate2<W, A>(W clazz) where A: Attribute where W: JAnnotationWriter<A>
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

        protected internal virtual bool isAnnotated
        {
            get
            {
                return _annotations != null;
            }
        }

        public virtual void bind(JFormatter f)
        {
            if (_annotations != null)
            {
                for (int i = 0; i < _annotations.Count; i++)
                {
                    f.g(_annotations[i]).nl();
                }
            }
            f.g(_mods).g(_type).id(_name);
            if (_init != null)
            {
                f.p('=').g(_init);
            }
        }

        public virtual void declare(JFormatter f)
        {
            f.b(this).p(';').nl();
        }

        public override void generate(JFormatter f)
        {
            f.id(_name);
        }


        public virtual JExpression assign(JExpression rhs)
        {
            return JExpr.assign(this, rhs);
        }
        public virtual JExpression assignPlus(JExpression rhs)
        {
            return JExpr.assignPlus(this, rhs);
        }

    }
}
