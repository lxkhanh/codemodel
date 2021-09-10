//-----------------------------------------------------------------------
// <copyright file="JPrimitiveType.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JPrimitiveType : JType
    {

        private readonly string typeName;
        private readonly JCodeModel _owner;
        /// <summary>
        /// Corresponding wrapper class.
        /// For example, this would be "java.lang.Short" for short.
        /// </summary>
        private readonly JClass wrapperClass;

        internal JPrimitiveType(JCodeModel owner, string typeName, Type wrapper)
        {
            this._owner = owner;
            this.typeName = typeName;
            this.wrapperClass = owner.@ref(wrapper);
        }

        public override JCodeModel owner()
        {
            return _owner;
        }

        public override string fullName()
        {
            return typeName;
        }

        public override string name()
        {
            return fullName();
        }

        public bool isPrimitive()
        {          
           return true;          
        }

        private JClass arrayClass;
        public override JClass array()
        {
            if (arrayClass == null)
            {
                arrayClass = new JArrayClass(_owner, this);
            }
            return arrayClass;
        }

        /// <summary>
        /// Obtains the wrapper class for this primitive type.
        /// For example, this method returns a reference to java.lang.Integer
        /// if this object represents int.
        /// </summary>
        public override JClass boxify()
        {
            return wrapperClass;
        }

        /// @deprecated calling this method from <seealso cref="JPrimitiveType"/>
        /// would be meaningless, since it's always guaranteed to
        /// return <tt>this</tt>. 
        public override JType unboxify()
        {
            return this;
        }

        /// <summary>
        /// @deprecated
        ///      Use <seealso cref="#boxify()"/>.
        /// </summary>
        public JClass WrapperClass
        {
            get
            {
                return boxify();
            }
        }

        /// <summary>
        /// Wraps an expression of this type to the corresponding wrapper class.
        /// For example, if this class represents "float", this method will return
        /// the expression <code>new Float(x)</code> for the paramter x.
        /// 
        /// REVISIT: it's not clear how this method works for VOID.
        /// </summary>
        public JExpression wrap(JExpression exp)
        {
            return JExpr._new(boxify()).arg(exp);
        }

        /// <summary>
        /// Do the opposite of the wrap method.
        /// 
        /// REVISIT: it's not clear how this method works for VOID.
        /// </summary>
        public JExpression unwrap(JExpression exp)
        {
            // it just so happens that the unwrap method is always
            // things like "intValue" or "booleanValue".
            return exp.invoke(typeName + "Value");
        }

        public override void generate(JFormatter f)
        {
            f.p(typeName);
        }
    }

}
