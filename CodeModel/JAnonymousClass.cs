//-----------------------------------------------------------------------
// <copyright file="JAnonymousClass.cs">
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
    /// Anonymous class quick hack.
    ///    
    internal class JAnonymousClass : JDefinedClass
    {

        /// <summary>
        /// Base interface/class from which this anonymous class is built.
        /// </summary>
        private readonly JClass _base;

        internal JAnonymousClass(JClass _base)
            : base(_base.owner(), 0, null)
        {
            this._base = _base;
        }

        public override string fullName()
        {
            return _base.fullName();
        }

        public override void generate(JFormatter f)
        {
            f.t(_base);
        }

    }
}
