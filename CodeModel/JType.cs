//-----------------------------------------------------------------------
// <copyright file="JType.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public abstract class JType : JGenerable, IComparable<JType>
    {
        public static JPrimitiveType parse(JCodeModel codeModel, String typeName)
        {
            if (typeName.Equals("void") || typeName.Equals("Void"))
            {
                return codeModel.VOID;
            }
            else if (typeName.Equals("boolean") || typeName.Equals("Boolean"))
            {
                return codeModel.BOOLEAN;
            }
            else if (typeName.Equals("byte") || typeName.Equals("Byte"))
            {
                return codeModel.BYTE;
            }
            else if (typeName.Equals("short") || typeName.Equals("Int16"))
            {
                return codeModel.SHORT;
            }
            else if (typeName.Equals("char") || typeName.Equals("Char"))
            {
                return codeModel.CHAR;
            }
            else if (typeName.Equals("int") || typeName.Equals("Int32"))
            {
                return codeModel.INT;
            }
            else if (typeName.Equals("float") || typeName.Equals("Single"))
            {
                return codeModel.FLOAT;
            }
            else if (typeName.Equals("long") || typeName.Equals("Int64"))
            {
                return codeModel.LONG;
            }
            else if (typeName.Equals("double") || typeName.Equals("Double"))
            {
                return codeModel.DOUBLE;
            }
            else
            {
                throw new ArgumentException(("Not a primitive type: " + typeName));
            }

        }

        public abstract JCodeModel owner();

        public abstract String fullName();

        public String binaryName()
        {
            return this.fullName();
        }

        public abstract String name();

        public abstract JClass array();

        public bool isArray()
        {
            return false;
        }

        public virtual bool isPrimitive()
        {
            return false;
        }

        public abstract JClass boxify();

        public abstract JType unboxify();

        public virtual JType erasure()
        {
            return this;
        }

        public bool isReference()
        {
            return !this.isPrimitive();
        }

        public virtual JType elementType()
        {
            throw new ArgumentException("Not an array type");     
        }

        public String toString()
        {
            return (this.GetType().Name+ ('('
                        + (this.fullName() + ')')));
        }

        public int CompareTo(JType o)
        {
            String rhs = o.fullName();
            bool p = this.fullName().StartsWith("java");
            bool q = rhs.StartsWith("java");
            if ((p && !q))
            {
                return -1;
            }
            else if ((!p && q))
            {
                return 1;
            }
            else
            {
                return this.fullName().CompareTo(rhs);
            }

        }

        public abstract void generate(JFormatter f);
    }
}
