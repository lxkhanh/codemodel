//-----------------------------------------------------------------------
// <copyright file="JArrayClass.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    internal sealed class JArrayClass : JClass
    {
        // array component type
        private readonly JType componentType;


        internal JArrayClass(JCodeModel owner, JType component)
            : base(owner)
        {
            this.componentType = component;
        }


        public override string name()
        {
            return componentType.name() + "[]";
        }

        public override string fullName()
        {
            return componentType.fullName() + "[]";
        }

        public string binaryName()
        {
            return componentType.binaryName() + "[]";
        }

        public override void generate(JFormatter f)
        {
            f.g(componentType).p("[]");
        }

        public override JPackage _package()
        {
            return owner().rootPackage();
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

        public override JType elementType()
        {
            return componentType;
        }

        public bool isArray
        {
            get
            {
                return true;
            }
        }


        //
        // Equality is based on value
        //

        public bool Equals(object obj)
        {
            if (!(obj is JArrayClass))
            {
                return false;
            }

            if (componentType.Equals(((JArrayClass)obj).componentType))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode()
        {
            return componentType.GetHashCode();
        }

        public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
        {
            if (componentType.isPrimitive())
            {
                return this;
            }

            JClass c = ((JClass)componentType).substituteParams(variables, bindings);
            if (c == componentType)
            {
                return this;
            }

            return new JArrayClass(owner(), c);
        }

    }
}
