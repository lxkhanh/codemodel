//-----------------------------------------------------------------------
// <copyright file="JDirectClass.cs">
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
    /// A special <seealso cref="JClass"/> that represents an unknown class (except its name.)
    /// 
    /// <seealso cref= JCodeModel#directClass(String)  </seealso>
    internal sealed class JDirectClass : JClass
    {

        private readonly string _fullName;

        public JDirectClass(JCodeModel _owner, string fullName)
            : base(_owner)
        {
            this._fullName = fullName;
        }

        public override string name()
        {
            int i = _fullName.LastIndexOf('.');
            if (i >= 0)
            {
                return _fullName.Substring(i + 1);
            }
            return _fullName;
        }

        public override string fullName()
        {
            return _fullName;
        }

        public override JPackage _package()
        {
            int i = _fullName.LastIndexOf('.');
            if (i >= 0)
            {
                return owner()._package(_fullName.Substring(0, i));
            }
            else
            {
                return owner().rootPackage();
            }
        }

        public override JClass _extends()
        {
            return owner().@ref(typeof(object));
        }

        public override IEnumerator<JClass> _implements()
        {
            return Enumerable.Empty<JClass>().GetEnumerator();
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
            return this;
        }
    }
}
