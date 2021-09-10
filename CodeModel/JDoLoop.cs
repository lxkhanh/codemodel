//-----------------------------------------------------------------------
// <copyright file="JDoLoop.cs">
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
    /// Do loops
    /// </summary>

    public class JDoLoop : JStatement
    {

        /// <summary>
        /// Test part of Do statement for determining exit state
        /// </summary>
        private JExpression test;

        /// <summary>
        /// JBlock of statements which makes up body of this Do statement
        /// </summary>       
        private JBlock _body = null;

        /// <summary>
        /// Construct a Do statment
        /// </summary>
        internal JDoLoop(JExpression test)
        {
            this.test = test;
        }

        public virtual JBlock body()
        {
            if (_body == null)
            {
                _body = new JBlock();
            }
            return _body;
        }

        public virtual void state(JFormatter f)
        {
            f.p("do");
            if (_body != null)
            {
                f.g(_body);
            }
            else
            {
                f.p("{ }");
            }

            if (JOp.hasTopOp(test))
            {
                f.p("while ").g(test);
            }
            else
            {
                f.p("while (").g(test).p(')');
            }
            f.p(';').nl();
        }

    }
}
