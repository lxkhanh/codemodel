//-----------------------------------------------------------------------
// <copyright file="JCatchBlock.cs">
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
    /// Catch block for a try/catch/finally statement
    /// </summary>

    public class JCatchBlock : JGenerable
    {

        internal JClass exception;
        private JVar var = null;
        private JBlock _body = new JBlock();

        internal JCatchBlock(JClass exception)
        {
            this.exception = exception;
        }

        public virtual JVar param(string name)
        {
            if (var != null)
            {
                throw new System.InvalidOperationException();
            }
            var = new JVar(JMods.forVar(JMod.NONE), exception, name, null);
            return var;
        }

        public virtual JBlock body()
        {
            return _body;
        }

        public virtual void generate(JFormatter f)
        {
            if (var == null)
            {
                var = new JVar(JMods.forVar(JMod.NONE), exception, "_x", null);
            }
            f.p("catch (").b(var).p(')').g(_body);
        }

    }
}
