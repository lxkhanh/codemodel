//-----------------------------------------------------------------------
// <copyright file="JStringLiteral.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public class JStringLiteral : JExpressionImpl
    {

        public readonly string str;


        internal JStringLiteral(string what)
        {
            this.str = what;

        }


        public override void generate(JFormatter f)
        {
            f.p(JExpr.quotify('"', str));
        }
    }
}
