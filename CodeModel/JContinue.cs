//-----------------------------------------------------------------------
// <copyright file="JContinue.cs">
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
    /// JContinue statement
    /// </summary>
    internal class JContinue : JStatement
    {

        private readonly JLabel label;

        /// <summary>
        /// JContinue constructor.
        /// </summary>
        /// <param name="_label">
        ///      a valid label or null. </param>
        internal JContinue(JLabel _label)
        {
            this.label = _label;
        }

        public virtual void state(JFormatter f)
        {
            if (label == null)
            {
                f.p("continue;").nl();
            }
            else
            {
                f.p("continue").p(label.label).p(';').nl();
            }
        }

    }
}
