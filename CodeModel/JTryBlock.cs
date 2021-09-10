//-----------------------------------------------------------------------
// <copyright file="JTryBlock.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public class JTryBlock : JStatement
    {    
        private JBlock _body = new JBlock();
        private IList<JCatchBlock> catches = new List<JCatchBlock>();
        private JBlock @finally = null;

        internal JTryBlock()
        {
        }

        public virtual JBlock body()
        {
            return _body;
        }

        public virtual JCatchBlock _catch(JClass exception)
        {
            JCatchBlock cb = new JCatchBlock(exception);
            catches.Add(cb);
            return cb;
        }

        public virtual JBlock _finally()
        {
            if (@finally == null)
            {
                @finally = new JBlock();
            }
            return @finally;
        }

        public virtual void state(JFormatter f)
        {
            f.p("try").g(_body);
            foreach (JCatchBlock cb in catches)
            {
                f.g(cb);
            }
            if (@finally != null)
            {
                f.p("finally").g(@finally);
            }
            f.nl();
        }

    }
}
