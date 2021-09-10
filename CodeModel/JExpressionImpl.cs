//-----------------------------------------------------------------------
// <copyright file="JExpressionImpl.cs">
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
    /// Provides default implementations for <seealso cref="JExpression"/>.
    /// </summary>
    public abstract class JExpressionImpl : JExpression
    {
        //
        //
        // from JOp
        //
        //
        public abstract void generate(JFormatter f);
        public JExpression minus()
        {
            return JOp.minus(this);
        }

        /// <summary>
        /// Logical not <tt>'!x'</tt>.
        /// </summary>
        public JExpression not()
        {
            return JOp.not(this);
        }

        public JExpression complement()
        {
            return JOp.complement(this);
        }

        public JExpression incr()
        {
            return JOp.incr(this);
        }

        public JExpression decr()
        {
            return JOp.decr(this);
        }

        public JExpression plus(JExpression right)
        {
            return JOp.plus(this, right);
        }

        public JExpression minus(JExpression right)
        {
            return JOp.minus(this, right);
        }

        public JExpression mul(JExpression right)
        {
            return JOp.mul(this, right);
        }

        public JExpression div(JExpression right)
        {
            return JOp.div(this, right);
        }

        public JExpression mod(JExpression right)
        {
            return JOp.mod(this, right);
        }

        public JExpression shl(JExpression right)
        {
            return JOp.shl(this, right);
        }

        public JExpression shr(JExpression right)
        {
            return JOp.shr(this, right);
        }

        public JExpression shrz(JExpression right)
        {
            return JOp.shrz(this, right);
        }

        public JExpression band(JExpression right)
        {
            return JOp.band(this, right);
        }

        public JExpression bor(JExpression right)
        {
            return JOp.bor(this, right);
        }

        public JExpression cand(JExpression right)
        {
            return JOp.cand(this, right);
        }

        public JExpression cor(JExpression right)
        {
            return JOp.cor(this, right);
        }

        public JExpression xor(JExpression right)
        {
            return JOp.xor(this, right);
        }

        public JExpression lt(JExpression right)
        {
            return JOp.lt(this, right);
        }

        public JExpression lte(JExpression right)
        {
            return JOp.lte(this, right);
        }

        public JExpression gt(JExpression right)
        {
            return JOp.gt(this, right);
        }

        public JExpression gte(JExpression right)
        {
            return JOp.gte(this, right);
        }

        public JExpression eq(JExpression right)
        {
            return JOp.eq(this, right);
        }

        public JExpression ne(JExpression right)
        {
            return JOp.ne(this, right);
        }

        public JExpression _instanceof(JType right)
        {
            return JOp._instanceof(this, right);
        }

        //
        //
        // from JExpr
        //
        //
        public JInvocation invoke(JMethod method)
        {
            return JExpr.invoke(this, method);
        }

        public JInvocation invoke(string method)
        {
            return JExpr.invoke(this, method);
        }

        public JFieldRef @ref(JVar field)
        {
            return JExpr.@ref(this, field);
        }

        public JFieldRef @ref(string field)
        {
            return JExpr.@ref(this, field);
        }

        public JArrayCompRef component(JExpression index)
        {
            return JExpr.component(this, index);
        }
    }

}
