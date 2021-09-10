//-----------------------------------------------------------------------
// <copyright file="JAtom.cs">
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
    /// JAtoms: Simple code components that merely generate themselves.
    /// </summary>
    internal sealed class JAtom : JExpressionImpl
    {

        private readonly string what;

        internal JAtom(string what)
        {
            this.what = what;
        }

        public override void generate(JFormatter f)
        {
            f.p(what);
        }
    }
}
