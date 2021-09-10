//-----------------------------------------------------------------------
// <copyright file="JTypeVar.cs">
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
    /// Type variable used to declare generics.
    /// </summary>
    /// <seealso cref= JGenerifiable
    /// @author
    ///     Kohsuke Kawaguchi (kohsuke.kawaguchi@sun.com) </seealso>
    public sealed class JTypeVar : JClass, JDeclaration
    {
        private readonly string _name;

        private JClass bound;

        internal JTypeVar(JCodeModel owner, string _name)
            : base(owner)
        {
            this._name = _name;
        }

        public override string name()
        {
            return _name;
        }

        public override string fullName()
        {
            return _name;
        }

        public override JPackage _package()
        {
            return null;
        }

        /// <summary>
        /// Adds a bound to this variable.
        /// </summary>
        /// <returns>  this </returns>
        public JTypeVar Bound(JClass c)
        {
            if (bound != null)
            {
                throw new System.ArgumentException("type variable has an existing class bound " + bound);
            }
            bound = c;
            return this;
        }

        /// <summary>
        /// Returns the class bound of this variable.
        /// 
        /// <p>
        /// If no bound is given, this method returns <seealso cref="Object"/>.
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

        /// <summary>
        /// Prints out the declaration of the variable.
        /// </summary>
        public void declare(JFormatter f)
        {
            f.id(_name);
            if (bound != null)
            {
                f.p("extends").g(bound);
            }
        }


        public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
        {
            for (int i = 0; i < variables.Length; i++)
            {
                if (variables[i] == this)
                {
                    return bindings[i];
                }
            }
            return this;
        }

        public override void generate(JFormatter f)
        {
            f.id(_name);
        }
    }
}
