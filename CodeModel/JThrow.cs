//-----------------------------------------------------------------------
// <copyright file="JThrow.cs">
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
    /// JThrow statement
    /// </summary>

    internal class JThrow : JStatement
    {

        /// <summary>
        /// JExpression to throw
        /// </summary>
        private JExpression expr;

        /// <summary>
        /// JThrow constructor
        /// </summary>
        /// <param name="expr">
        ///        JExpression which evaluates to JThrow value </param>
        internal JThrow(JExpression expr)
        {
            this.expr = expr;
        }

        public virtual void state(JFormatter f)
        {
            f.p("throw");
            f.g(expr);
            f.p(';').nl();
        }

    }
}
