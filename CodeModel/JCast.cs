//-----------------------------------------------------------------------
// <copyright file="JCast.cs">
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
    /// A cast operation.
    /// </summary>
    public sealed class JCast : JExpressionImpl
    {
        /// <summary>
        /// JType to which the expression is to be cast.
        /// </summary>
        private readonly JType type;

        /// <summary>
        /// JExpression to be cast.
        /// </summary>
        private readonly JExpression @object;

        /// <summary>
        /// JCast constructor 
        /// </summary>
        /// <param name="type">
        ///        JType to which the expression is cast
        /// </param>
        /// <param name="object">
        ///        JExpression for the object upon which
        ///        the cast is applied </param>
        internal JCast(JType type, JExpression @object)
        {
            this.type = type;
            this.@object = @object;
        }

        public override void generate(JFormatter f)
        {
            f.p("((").g(type).p(')').g(@object).p(')');
        }
    }
}
