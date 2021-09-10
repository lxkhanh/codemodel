//-----------------------------------------------------------------------
// <copyright file="JOp.cs">
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
    /// JClass for generating expressions containing operators
    /// </summary>

    public abstract class JOp
    {

        private JOp()
        {
        }


        /// <summary>
        /// Determine whether the top level of an expression involves an
        /// operator.
        /// </summary>
        internal static bool hasTopOp(JExpression e)
        {
            return (e is UnaryOp) || (e is BinaryOp);
        }

        /* -- Unary operators -- */

        private class UnaryOp : JExpressionImpl
        {

            protected internal string op;
            protected internal JExpression e;
            protected internal bool opFirst = true;

            public UnaryOp(string op, JExpression e)
            {
                this.op = op;
                this.e = e;
            }

            public UnaryOp(JExpression e, string op)
            {
                this.op = op;
                this.e = e;
                opFirst = false;
            }

            public override void generate(JFormatter f)
            {
                if (opFirst)
                {
                    f.p('(').p(op).g(e).p(')');
                }
                else
                {
                    f.p('(').g(e).p(op).p(')');
                }
            }

        }

        public static JExpression minus(JExpression e)
        {
            return new UnaryOp("-", e);
        }

        /// <summary>
        /// Logical not <tt>'!x'</tt>.
        /// </summary>
        public static JExpression not(JExpression e)
        {
            if (e == JExpr.TRUE)
            {
                return JExpr.FALSE;
            }
            if (e == JExpr.FALSE)
            {
                return JExpr.TRUE;
            }
            return new UnaryOp("!", e);
        }

        public static JExpression complement(JExpression e)
        {
            return new UnaryOp("~", e);
        }

        private class TightUnaryOp : UnaryOp
        {

            internal TightUnaryOp(JExpression e, string op)
                : base(e, op)
            {
            }

            public void generate(JFormatter f)
            {
                if (opFirst)
                {
                    f.p(op).g(e);
                }
                else
                {
                    f.g(e).p(op);
                }
            }

        }

        public static JExpression incr(JExpression e)
        {
            return new TightUnaryOp(e, "++");
        }

        public static JExpression decr(JExpression e)
        {
            return new TightUnaryOp(e, "--");
        }


        /* -- Binary operators -- */

        private class BinaryOp : JExpressionImpl
        {

            private string op;
            private JExpression left;
            private JGenerable right;

            public BinaryOp(string op, JExpression left, JGenerable right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override void generate(JFormatter f)
            {
                f.p('(').g(left).p(op).g(right).p(')');
            }

        }

        public static JExpression plus(JExpression left, JExpression right)
        {
            return new BinaryOp("+", left, right);
        }

        public static JExpression minus(JExpression left, JExpression right)
        {
            return new BinaryOp("-", left, right);
        }

        public static JExpression mul(JExpression left, JExpression right)
        {
            return new BinaryOp("*", left, right);
        }

        public static JExpression div(JExpression left, JExpression right)
        {
            return new BinaryOp("/", left, right);
        }

        public static JExpression mod(JExpression left, JExpression right)
        {
            return new BinaryOp("%", left, right);
        }

        public static JExpression shl(JExpression left, JExpression right)
        {
            return new BinaryOp("<<", left, right);
        }

        public static JExpression shr(JExpression left, JExpression right)
        {
            return new BinaryOp(">>", left, right);
        }

        public static JExpression shrz(JExpression left, JExpression right)
        {
            return new BinaryOp(">>>", left, right);
        }

        public static JExpression band(JExpression left, JExpression right)
        {
            return new BinaryOp("&", left, right);
        }

        public static JExpression bor(JExpression left, JExpression right)
        {
            return new BinaryOp("|", left, right);
        }

        public static JExpression cand(JExpression left, JExpression right)
        {
            if (left == JExpr.TRUE)
            {
                return right;
            }
            if (right == JExpr.TRUE)
            {
                return left;
            }
            if (left == JExpr.FALSE) // JExpr.FALSE
            {
                return left;
            }
            if (right == JExpr.FALSE) // JExpr.FALSE
            {
                return right;
            }
            return new BinaryOp("&&", left, right);
        }

        public static JExpression cor(JExpression left, JExpression right)
        {
            if (left == JExpr.TRUE) // JExpr.TRUE
            {
                return left;
            }
            if (right == JExpr.TRUE) // JExpr.FALSE
            {
                return right;
            }
            if (left == JExpr.FALSE)
            {
                return right;
            }
            if (right == JExpr.FALSE)
            {
                return left;
            }
            return new BinaryOp("||", left, right);
        }

        public static JExpression xor(JExpression left, JExpression right)
        {
            return new BinaryOp("^", left, right);
        }

        public static JExpression lt(JExpression left, JExpression right)
        {
            return new BinaryOp("<", left, right);
        }

        public static JExpression lte(JExpression left, JExpression right)
        {
            return new BinaryOp("<=", left, right);
        }

        public static JExpression gt(JExpression left, JExpression right)
        {
            return new BinaryOp(">", left, right);
        }

        public static JExpression gte(JExpression left, JExpression right)
        {
            return new BinaryOp(">=", left, right);
        }

        public static JExpression eq(JExpression left, JExpression right)
        {
            return new BinaryOp("==", left, right);
        }

        public static JExpression ne(JExpression left, JExpression right)
        {
            return new BinaryOp("!=", left, right);
        }

        public static JExpression _instanceof(JExpression left, JType right)
        {
            return new BinaryOp("instanceof", left, right);
        }

        /* -- Ternary operators -- */

        private class TernaryOp : JExpressionImpl
        {

            private string op1;
            private string op2;
            private JExpression e1;
            private JExpression e2;
            private JExpression e3;

            public TernaryOp(string op1, string op2, JExpression e1, JExpression e2, JExpression e3)
            {
                this.e1 = e1;
                this.op1 = op1;
                this.e2 = e2;
                this.op2 = op2;
                this.e3 = e3;
            }

            public override void generate(JFormatter f)
            {
                f.p('(').g(e1).p(op1).g(e2).p(op2).g(e3).p(')');
            }

        }

        public static JExpression cond(JExpression cond, JExpression ifTrue, JExpression ifFalse)
        {
            return new TernaryOp("?", ":", cond, ifTrue, ifFalse);
        }

    }
}
