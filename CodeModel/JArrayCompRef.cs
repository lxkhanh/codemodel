//-----------------------------------------------------------------------
// <copyright file="JArrayCompRef.cs">
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
    /// array component reference.
    /// </summary>
    public sealed class JArrayCompRef : JExpressionImpl, JAssignmentTarget
    {
        /// <summary>
        /// JArray expression upon which this component will be accessed.
        /// </summary>
        private readonly JExpression array;

        /// <summary>
        /// Integer expression representing index of the component
        /// </summary>
        private readonly JExpression index;

        /// <summary>
        /// JArray component reference constructor given an array expression
        /// and index.
        /// </summary>
        /// <param name="array">
        ///        JExpression for the array upon which
        ///        the component will be accessed,
        /// </param>
        /// <param name="index">
        ///        JExpression for index of component to access </param>
        internal JArrayCompRef(JExpression array, JExpression index)
        {
            if ((array == null) || (index == null))
            {
                throw new System.NullReferenceException();
            }
            this.array = array;
            this.index = index;
        }

        public override void generate(JFormatter f)
        {
            f.g(array).p('[').g(index).p(']');
        }

        public JExpression assign(JExpression rhs)
        {
            return JExpr.assign(this, rhs);
        }
        public JExpression assignPlus(JExpression rhs)
        {
            return JExpr.assignPlus(this, rhs);
        }
    }
}
