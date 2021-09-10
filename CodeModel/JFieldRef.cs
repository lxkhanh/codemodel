//-----------------------------------------------------------------------
// <copyright file="JFieldRef.cs">
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
    /// Field Reference
    /// </summary>

    public class JFieldRef : JExpressionImpl, JAssignmentTarget
    {
        /// <summary>
        /// Object expression upon which this field will be accessed, or
        /// null for the implicit 'this'.
        /// </summary>
        private JGenerable @object;

        /// <summary>
        /// Name of the field to be accessed. Either this or <seealso cref="#var"/> is set.
        /// </summary>
        private string name;

        /// <summary>
        /// Variable to be accessed.
        /// </summary>
        private JVar var;

        /// <summary>
        /// Indicates if an explicit this should be generated
        /// </summary>
        private bool explicitThis;

        /// <summary>
        /// Field reference constructor given an object expression and field name
        /// </summary>
        /// <param name="object">
        ///        JExpression for the object upon which
        ///        the named field will be accessed,
        /// </param>
        /// <param name="name">
        ///        Name of field to access </param>
        internal JFieldRef(JExpression @object, string name)
            : this(@object, name, false)
        {
        }

        internal JFieldRef(JExpression @object, JVar v)
            : this(@object, v, false)
        {
        }

        /// <summary>
        /// Static field reference.
        /// </summary>
        internal JFieldRef(JType type, string name)
            : this(type, name, false)
        {
        }

        internal JFieldRef(JType type, JVar v)
            : this(type, v, false)
        {
        }

        internal JFieldRef(JGenerable @object, string name, bool explicitThis)
        {
            this.explicitThis = explicitThis;
            this.@object = @object;
            if (name.IndexOf('.') >= 0)
            {
                throw new System.ArgumentException("Field name contains '.': " + name);
            }
            this.name = name;
        }

        internal JFieldRef(JGenerable @object, JVar var, bool explicitThis)
        {
            this.explicitThis = explicitThis;
            this.@object = @object;
            this.var = var;
        }

        public override void generate(JFormatter f)
        {
            string name = this.name;
            if (string.ReferenceEquals(name, null))
            {
                name = var.name();
            }

            if (@object != null)
            {
                f.g(@object).p('.').p(name);
            }
            else
            {
                if (explicitThis)
                {
                    f.p("this.").p(name);
                }
                else
                {
                    f.id(name);
                }
            }
        }

        public virtual JExpression assign(JExpression rhs)
        {
            return JExpr.assign(this, rhs);
        }
        public virtual JExpression assignPlus(JExpression rhs)
        {
            return JExpr.assignPlus(this, rhs);
        }
    }
}
