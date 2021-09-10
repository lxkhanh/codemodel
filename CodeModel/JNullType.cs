//-----------------------------------------------------------------------
// <copyright file="JNullType.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JNullType : JClass
    {
        public JNullType(JCodeModel _owner)
            : base(_owner)
        {
        }

        public override String name()
        {
            return "null";
        }

        public override String fullName()
        {
            return "null";
        }

        public override JPackage _package()
        {
            return owner()._package("");
        }

        public override JClass _extends()
        {
            return null;
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
