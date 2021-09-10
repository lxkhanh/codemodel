//-----------------------------------------------------------------------
// <copyright file="JTypeWildcard.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    internal sealed class JTypeWildcard : JClass
    {

        private readonly JClass bound;

        internal JTypeWildcard(JClass bound)
            : base(bound.owner())
        {
            this.bound = bound;
        }

        public override string name()
        {
            return "? extends " + bound.name();
        }

        public override string fullName()
        {
            return "? extends " + bound.fullName();
        }

        public override JPackage _package()
        {
            return null;
        }

        /// <summary>
        /// Returns the class bound of this variable.
        /// 
        /// <para>
        /// If no bound is given, this method returns <seealso cref="Object"/>.
        /// </para>
        /// </summary>
        public override JClass _extends()
        {
            if (bound != null)
            {
                return bound;
            }
            else
            {
                return owner().@ref(typeof(object));
            }
        }

        /// <summary>
        /// Returns the interface bounds of this variable.
        /// </summary>
        public override IEnumerator<JClass> _implements()
        {
            return bound._implements();
        }

        public override bool isInterface
        {
            get
            {
                return false;
            }
        }

        public override bool isAbstract
        {
            get
            {
                return false;
            }
        }

        public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
        {
            JClass nb = bound.substituteParams(variables, bindings);
            if (nb == bound)
            {
                return this;
            }
            else
            {
                return new JTypeWildcard(nb);
            }
        }

        public override void generate(JFormatter f)
        {
            if (bound._extends() == null)
            {
                f.p("?"); // instead of "? extends Object"
            }
            else
            {
                f.p("? extends").g(bound);
            }
        }
    }
}
