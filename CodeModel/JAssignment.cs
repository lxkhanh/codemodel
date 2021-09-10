//-----------------------------------------------------------------------
// <copyright file="JAssignment.cs">
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
    /// Assignment statements, which are also expressions.
    /// </summary>
    public class JAssignment : JExpressionImpl, JStatement
    {
        private JAssignmentTarget lhs;
        private JExpression rhs;
        private string op = "";

        public JAssignment(JAssignmentTarget lhs, JExpression rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public JAssignment(JAssignmentTarget lhs, JExpression rhs, string op)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        public override void generate(JFormatter f)
        {
            f.g(lhs).p(op + '=').g(rhs);
        }

        public void state(JFormatter f)
        {
            f.g(this).p(';').nl();
        }

    }
}
