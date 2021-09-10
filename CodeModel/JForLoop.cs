//-----------------------------------------------------------------------
// <copyright file="JForLoop.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public class JForLoop : JStatement
    {
        private IList<object> inits = new List<object>();
        private JExpression _test = null;
        private IList<JExpression> updates = new List<JExpression>();

        private JBlock _body = null;

        public virtual JVar init(int mods, JType type, string var, JExpression e)
        {
            JVar v = new JVar(JMods.forVar(mods), type, var, e);
            inits.Add(v);
            return v;
        }

        public virtual JVar init(JType type, string var, JExpression e)
        {
            return init(JMod.NONE, type, var, e);
        }

        public virtual void init(JVar v, JExpression e)
        {
            inits.Add(JExpr.assign(v, e));
        }

        public virtual void test(JExpression e)
        {
            this._test = e;
        }

        public virtual void update(JExpression e)
        {
            updates.Add(e);
        }

        public virtual JBlock body()
        {
            if (_body == null)
            {
                _body = new JBlock();
            }
            return _body;
        }

        public virtual void state(JFormatter f)
        {
            f.p("for (");
            bool first = true;
            foreach (object o in inits)
            {
                if (!first)
                {
                    f.p(',');
                }
                if (o is JVar)
                {
                    f.b((JVar)o);
                }
                else
                {
                    f.g((JExpression)o);
                }
                first = false;
            }
            f.p(';').g(_test).p(';').g(updates).p(')');
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
