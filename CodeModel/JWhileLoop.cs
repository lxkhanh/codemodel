//-----------------------------------------------------------------------
// <copyright file="JWhileLoop.cs">
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
    /// While statement
    /// </summary>

    public class JWhileLoop : JStatement
    {

        /// <summary>
        /// Test part of While statement for determining exit state
        /// </summary>     
        private JExpression _test;

        /// <summary>
        /// JBlock of statements which makes up body of this While statement
        /// </summary>       
        private JBlock _body = null;

        /// <summary>
        /// Construct a While statment
        /// </summary>
        internal JWhileLoop(JExpression test)
        {
            this._test = test;
        }

        public virtual JExpression test()
        {
            return _test;
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
            if (JOp.hasTopOp(_test))
            {
                f.p("while ").g(_test);
            }
            else
            {
                f.p("while (").g(_test).p(')');
            }
            if (_body != null)
            {
                f.s(_body);
            }
            else
            {
                f.p(';').nl();
            }
        }

    }
}
