//-----------------------------------------------------------------------
// <copyright file="JLabel.cs">
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
    /// Label that can be used for continue and break.   
    /// </summary>
    public class JLabel : JStatement
    {
        internal readonly string label;

        /// <summary>
        /// JBreak constructor
        /// </summary>
        /// <param name="_label">
        ///      break label or null. </param>
        internal JLabel(string _label)
        {
            this.label = _label;
        }

        public virtual void state(JFormatter f)
        {
            f.p(label + ':').nl();
        }

    }

}
