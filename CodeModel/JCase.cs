//-----------------------------------------------------------------------
// <copyright file="JCase.cs">
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
    /// Case statement
    /// </summary>
    public sealed class JCase : JStatement
    {

        /// <summary>
        /// label part of the case statement 
        /// </summary>
        private JExpression _label;

        /// <summary>
        /// JBlock of statements which makes up body of this While statement
        /// </summary>       
        private JBlock _body = null;

        /// <summary>
        /// is this a regular case statement or a default case statement?
        /// </summary>
        private bool isDefaultCase = false;

        /// <summary>
        /// Construct a case statement
        /// </summary>
        internal JCase(JExpression label)
            : this(label, false)
        {
        }

        /// <summary>
        /// Construct a case statement.  If isDefaultCase is true, then
        /// label should be null since default cases don't have a label.
        /// </summary>
        internal JCase(JExpression label, bool isDefaultCase)
        {
            this._label = label;
            this.isDefaultCase = isDefaultCase;
        }

        public JExpression label()
        {
            return _label;
        }

        public JBlock body()
        {
            if (_body == null)
            {
                _body = new JBlock(false, true);
            }
            return _body;
        }

        public void state(JFormatter f)
        {
            f.i();
            if (!isDefaultCase)
            {
                f.p("case ").g(_label).p(':').nl();
            }
            else
            {
                f.p("default:").nl();
            }
            if (_body != null)
            {
                f.s(_body);
            }
            f.o();
        }
    }
}
