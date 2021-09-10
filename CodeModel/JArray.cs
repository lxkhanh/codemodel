//-----------------------------------------------------------------------
// <copyright file="JArray.cs">
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
    /// array creation and initialization.
    /// </summary>
    public sealed class JArray : JExpressionImpl
    {

        private readonly JType type;
        private readonly JExpression size;
        private IList<JExpression> exprs = null;

        /// <summary>
        /// Add an element to the array initializer
        /// </summary>
        public JArray add(JExpression e)
        {
            if (exprs == null)
            {
                exprs = new List<JExpression>();
            }
            exprs.Add(e);
            return this;
        }

        internal JArray(JType type, JExpression size)
        {
            this.type = type;
            this.size = size;
        }

        public override void generate(JFormatter f)
        {

            // generally we produce new T[x], but when T is an array type (T=T'[])
            // then new T'[][x] is wrong. It has to be new T'[x][].
            int arrayCount = 0;
            JType t = type;

            while (t.isArray())
            {
                t = t.elementType();
                arrayCount++;
            }

            f.p("new").g(t).p('[');
            if (size != null)
            {
                f.g(size);
            }
            f.p(']');

            for (int i = 0; i < arrayCount; i++)
            {
                f.p("[]");
            }

            if ((size == null) || (exprs != null))
            {
                f.p('{');
            }
            if (exprs != null)
            {
                f.g(exprs);
            }
            else
            {
                f.p(' ');
            }
            if ((size == null) || (exprs != null))
            {
                f.p('}');
            }
        }

    }
}
