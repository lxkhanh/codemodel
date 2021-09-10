//-----------------------------------------------------------------------
// <copyright file="JGenerifiableImpl.cs">
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
    /// Implementation of <seealso cref="JGenerifiable"/>.     
    /// </summary>
    public abstract class JGenerifiableImpl : JGenerifiable, JDeclaration
    {
        /// <summary>
        /// Lazily created list of <seealso cref="JTypeVar"/>s. </summary>
        private IList<JTypeVar> typeVariables = null;

        protected internal abstract JCodeModel owner();

        public virtual void declare(JFormatter f)
        {
            if (typeVariables != null)
            {
                f.p('<');
                for (int i = 0; i < typeVariables.Count; i++)
                {
                    if (i != 0)
                    {
                        f.p(',');
                    }
                    f.d(typeVariables[i]);
                }
                f.p('>');
            }
        }


        public virtual JTypeVar generify(string name)
        {
            JTypeVar v = new JTypeVar(owner(), name);
            if (typeVariables == null)
            {
                typeVariables = new List<JTypeVar>(3);
            }
            typeVariables.Add(v);
            return v;
        }

        public virtual JTypeVar generify(string name, Type bound)
        {
            return generify(name, owner().@ref(bound));
        }

        public virtual JTypeVar generify(string name, JClass bound)
        {
            return generify(name).Bound(bound);
        }

        public virtual JTypeVar[] typeParams()
        {
            if (typeVariables == null)
            {
                return JTypeVar.EMPTY_ARRAY;
            }
            else
            {
                return typeVariables.ToArray();
            }
        }

    }
}
