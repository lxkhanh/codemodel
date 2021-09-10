//-----------------------------------------------------------------------
// <copyright file="JReturn.cs">
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
    /// A return statement
    /// </summary>
    internal class JReturn : JStatement
    {

        /// <summary>
        /// JExpression to return; may be null.
        /// </summary>
        private JExpression expr;

        /// <summary>
        /// JReturn constructor
        /// </summary>
        /// <param name="expr">
        ///        JExpression which evaluates to return value </param>
        internal JReturn(JExpression expr)
        {
            this.expr = expr;
        }

        public virtual void state(JFormatter f)
        {
            f.p("return ");
            if (expr != null)
            {
                f.g(expr);
            }
            f.p(';').nl();
        }

    }
}
