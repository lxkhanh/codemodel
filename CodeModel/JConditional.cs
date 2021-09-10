//-----------------------------------------------------------------------
// <copyright file="JConditional.cs">
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
    /// If statement, with optional else clause
    /// </summary>

    public class JConditional : JStatement
    {

        /// <summary>
        /// JExpression to test to determine branching
        /// </summary>
        private JExpression test = null;

        /// <summary>
        /// JBlock of statements for "then" clause
        /// </summary>        
        private JBlock then = new JBlock();

        /// <summary>
        /// JBlock of statements for optional "else" clause
        /// </summary>      
        private JBlock @else = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="test">
        ///        JExpression which will determine branching </param>
        internal JConditional(JExpression test)
        {
            this.test = test;
        }

        /// <summary>
        /// Return the block to be excuted by the "then" branch
        /// </summary>
        /// <returns> Then block </returns>
        public virtual JBlock _then()
        {
            return then;
        }

        /// <summary>
        /// Create a block to be executed by "else" branch
        /// </summary>
        /// <returns> Newly generated else block </returns>
        public virtual JBlock _else()
        {
            if (@else == null)
            {
                @else = new JBlock();
            }
            return @else;
        }

        /// <summary>
        /// Creates <tt>... else if(...) ...</tt> code.
        /// </summary>
        public virtual JConditional _elseif(JExpression boolExp)
        {
            return _else()._if(boolExp);
        }

        public virtual void state(JFormatter f)
        {
            if (test == JExpr.TRUE)
            {
                then.generateBody(f);
                return;
            }
            if (test == JExpr.FALSE)
            {
                @else.generateBody(f);
                return;
            }

            if (JOp.hasTopOp(test))
            {
                f.p("if ").g(test);
            }
            else
            {
                f.p("if (").g(test).p(')');
            }
            f.g(then);
            if (@else != null)
            {
                f.p("else").g(@else);
            }
            f.nl();
        }
    }
}
