//-----------------------------------------------------------------------
// <copyright file="JAnnotationStringValue.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JAnnotationStringValue : JAnnotationValue
    {

        private JExpression value;

        public JAnnotationStringValue(JExpression value)
        {
            this.value = value;
        }

        public override void generate(JFormatter f)
        {
            f.g(this.value);
        }
    }
}
