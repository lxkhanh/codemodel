//-----------------------------------------------------------------------
// <copyright file="JBreak.cs">
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
    /// JBreak statement
    /// </summary>
    internal sealed class JBreak : JStatement
    {

        private readonly JLabel label;

        /// <summary>
        /// JBreak constructor
        /// </summary>
        /// <param name="_label">
        ///      break label or null. </param>
        internal JBreak(JLabel _label)
        {
            this.label = _label;
        }

        public void state(JFormatter f)
        {
            if (label == null)
            {
                f.p("break;").nl();
            }
            else
            {
                f.p("break").p(label.label).p(';').nl();
            }
        }
    }
}
