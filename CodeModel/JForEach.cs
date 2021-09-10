//-----------------------------------------------------------------------
// <copyright file="JForEach.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JForEach : JStatement
    {

        private readonly JType type;      
        private readonly string _var;     
        private JBlock _body = null; // lazily created
        private readonly JExpression collection;
        private readonly JVar loopVar;

        public JForEach(JType vartype, string variable, JExpression collection)
        {

            this.type = vartype;
            this._var = variable;
            this.collection = collection;
            loopVar = new JVar(JMods.forVar(JMod.NONE), type, _var, collection);
        }


        /// <summary>
        /// Returns a reference to the loop variable.
        /// </summary>
        public JVar var()
        {
            return loopVar;
        }

        public JBlock body()
        {
            if (_body == null)
            {
                _body = new JBlock();
            }
            return _body;
        }

        public void state(JFormatter f)
        {
            f.p("for (");
            f.g(type).id(_var).p(": ").g(collection);
            f.p(')');
            if (_body != null)
            {
                f.g(_body).nl();
            }
            else
            {
                f.p(';').nl();
            }
        }

    }
}
