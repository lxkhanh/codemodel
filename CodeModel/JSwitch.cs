//-----------------------------------------------------------------------
// <copyright file="JSwitch.cs">
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
    /// Switch statement
    /// </summary>
    public sealed class JSwitch : JStatement
    {

        /// <summary>
        /// Test part of switch statement.
        /// </summary>       
        private JExpression _test;

        /// <summary>
        /// vector of JCases.
        /// </summary>    
        private IList<JCase> _cases = new List<JCase>();

        /// <summary>
        /// a single default case
        /// </summary>
        private JCase defaultCase = null;

        /// <summary>
        /// Construct a While statment
        /// </summary>
        internal JSwitch(JExpression test)
        {
            this._test = test;
        }

        public JExpression test()
        {
            return _test;
        }

        public IEnumerator<JCase> cases()
        {
            return _cases.GetEnumerator();
        }

        public JCase _case(JExpression label)
        {
            JCase c = new JCase(label);
            _cases.Add(c);
            return c;
        }

        public JCase _default()
        {
            // what if (default != null) ???

            // default cases statements don't have a label
            defaultCase = new JCase(null, true);
            return defaultCase;
        }

        public void state(JFormatter f)
        {
            if (JOp.hasTopOp(_test))
            {
                f.p("switch ").g(_test).p(" {").nl();
            }
            else
            {
                f.p("switch (").g(_test).p(')').p(" {").nl();
            }
            foreach (JCase c in _cases)
            {
                f.s(c);
            }
            if (defaultCase != null)
            {
                f.s(defaultCase);
            }
            f.p('}').nl();
        }

    }
}
