//-----------------------------------------------------------------------
// <copyright file="JExpr.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using CodeModel.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    /// <summary>
    /// Factory methods that generate various <seealso cref="JExpression"/>s.
    /// </summary>
    public abstract class JExpr
    {

        /// <summary>
        /// This class is not instanciable.
        /// </summary>
        private JExpr()
        {
        }

        public static JExpression assign(JAssignmentTarget lhs, JExpression rhs)
        {
            return new JAssignment(lhs, rhs);
        }

        public static JExpression assignPlus(JAssignmentTarget lhs, JExpression rhs)
        {
            return new JAssignment(lhs, rhs, "+");
        }

        public static JInvocation _new(JClass c)
        {
            return new JInvocation(c);
        }

        public static JInvocation _new(JType t)
        {
            return new JInvocation(t);
        }

        public static JInvocation invoke(string method)
        {
            return new JInvocation((JExpression)null, method);
        }

        public static JInvocation invoke(JMethod method)
        {
            return new JInvocation((JExpression)null, method);
        }

        public static JInvocation invoke(JExpression lhs, JMethod method)
        {
            return new JInvocation(lhs, method);
        }

        public static JInvocation invoke(JExpression lhs, string method)
        {
            return new JInvocation(lhs, method);
        }

        public static JFieldRef @ref(string field)
        {
            return new JFieldRef((JExpression)null, field);
        }

        public static JFieldRef @ref(JExpression lhs, JVar field)
        {
            return new JFieldRef(lhs, field);
        }

        public static JFieldRef @ref(JExpression lhs, string field)
        {
            return new JFieldRef(lhs, field);
        }

        public static JFieldRef refthis(string field)
        {
            return new JFieldRef(null, field, true);
        }
      
        public static JExpression dotclass(JClass cl)
        {
            return new JExpressionImplAnonymousInnerClass(cl);
        }

        private class JExpressionImplAnonymousInnerClass : JExpressionImpl
        {
            private JClass cl;

            public JExpressionImplAnonymousInnerClass(JClass cl)
            {
                this.cl = cl;
            }

            public override void generate(JFormatter f)
            {
                JClass c;
                if (cl is JNarrowedClass)
                {
                    c = ((JNarrowedClass)cl).basis;
                }
                else
                {
                    c = cl;
                }
                f.g(c).p(".class");
            }
        }

        public static JArrayCompRef component(JExpression lhs, JExpression index)
        {
            return new JArrayCompRef(lhs, index);
        }

        public static JCast cast(JType type, JExpression expr)
        {
            return new JCast(type, expr);
        }

        public static JArray newArray(JType type)
        {
            return newArray(type, null);
        }

        /// <summary>
        /// Generates {@code new T[size]}.
        /// </summary>
        /// <param name="type">
        ///      The type of the array component. 'T' or {@code new T[size]}. </param>
        public static JArray newArray(JType type, JExpression size)
        {
            // you cannot create an array whose component type is a generic
            return new JArray(type.erasure(), size);
        }

        /// <summary>
        /// Generates {@code new T[size]}.
        /// </summary>
        /// <param name="type">
        ///      The type of the array component. 'T' or {@code new T[size]}. </param>
        public static JArray newArray(JType type, int size)
        {
            return newArray(type, lit(size));
        }


        private static readonly JExpression __this = new JAtom("this");
        /// <summary>
        /// Returns a reference to "this", an implicit reference
        /// to the current object.
        /// </summary>
        public static JExpression _this()
        {
            return __this;
        }

        private static readonly JExpression __super = new JAtom("super");
        /// <summary>
        /// Returns a reference to "super", an implicit reference
        /// to the super class.
        /// </summary>
        public static JExpression _super()
        {
            return __super;
        }


        /* -- Literals -- */

        private static readonly JExpression __null = new JAtom("null");
        public static JExpression _null()
        {
            return __null;
        }

        /// <summary>
        /// Boolean constant that represents <code>true</code>
        /// </summary>
        public static readonly JExpression TRUE = new JAtom("true");

        /// <summary>
        /// Boolean constant that represents <code>false</code>
        /// </summary>
        public static readonly JExpression FALSE = new JAtom("false");

        public static JExpression lit(bool b)
        {
            return b ? TRUE : FALSE;
        }

        public static JExpression lit(int n)
        {
            return new JAtom(Convert.ToString(n));
        }

        public static JExpression lit(long n)
        {
            return new JAtom(Convert.ToString(n) + "L");
        }

        public static JExpression lit(float f)
        {         
            if (float.IsNegativeInfinity(f))
            {
                return new JAtom("java.lang.Float.NEGATIVE_INFINITY");
            }
            else if (float.IsPositiveInfinity(f))
            {
                return new JAtom("java.lang.Float.POSITIVE_INFINITY");
            }
            else if (float.IsNaN(f))
            {
                return new JAtom("java.lang.Float.NaN");
            }
            else
            {
                return new JAtom(f.ToString() + "F");
            }
        }

        public static JExpression lit(double d)
        {
            if (MathUtil.IsNegativeZero(d))
            {
                return new JAtom("-0.0D");
            }
            else if (MathUtil.IsPositiveZero(d)) {
                return new JAtom("0.0D");
            }
            else if (double.IsNegativeInfinity(d))
            {
                return new JAtom("java.lang.Double.NEGATIVE_INFINITY");
            }
            else if (double.IsPositiveInfinity(d))
            {
                return new JAtom("java.lang.Double.POSITIVE_INFINITY");
            }
            else if (double.IsNaN(d))
            {
                return new JAtom("java.lang.Double.NaN");
            }
            else
            {
                System.Globalization.NumberFormatInfo numberFormatInfo = new System.Globalization.NumberFormatInfo();
                numberFormatInfo.NumberDecimalSeparator = ".";
                return new JAtom(d.ToString("R", numberFormatInfo) + "D");
            }
        }

        internal const string charEscape = "\b\t\n\f\r\"\'\\";
        internal const string charMacro = "btnfr\"'\\";

        /// <summary>
        /// Escapes the given string, then surrounds it by the specified
        /// quotation mark. 
        /// </summary>
        public static string quotify(char quote, string s)
        {
            int n = s.Length;
            StringBuilder sb = new StringBuilder(n + 2);
            sb.Append(quote);
            for (int i = 0; i < n; i++)
            {
                char c = s[i];
                int j = charEscape.IndexOf(c);
                if (j >= 0)
                {
                    if ((quote == '"' && c == '\'') || (quote == '\'' && c == '"'))
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append('\\');
                        sb.Append(charMacro[j]);
                    }
                }
                else
                {
                    // technically Unicode escape shouldn't be done here,
                    // for it's a lexical level handling.
                    // 
                    // However, various tools are so broken around this area,
                    // so just to be on the safe side, it's better to do
                    // the escaping here (regardless of the actual file encoding)
                    //
                    // see bug 
                    if (c < (char)0x20 || (char)0x7E < c)
                    {
                        // not printable. use Unicode escape
                        sb.Append("\\u");
                        string hex = (((int)c) & 0xFFFF).ToString("x");
                        for (int k = hex.Length; k < 4; k++)
                        {
                            sb.Append('0');
                        }
                        sb.Append(hex);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            sb.Append(quote);
            return sb.ToString();
        }

        public static JExpression lit(char c)
        {
            return new JAtom(quotify('\'', "" + c));
        }

        public static JExpression lit(string s)
        {
            return new JStringLiteral(s);
        }

        /// <summary>
        /// Creates an expression directly from a source code fragment.
        /// 
        /// <para>
        /// This method can be used as a short-cut to create a JExpression.
        /// For example, instead of <code>_a.gt(_b)</code>, you can write
        /// it as: <code>JExpr.direct("a>b")</code>.
        /// 
        /// </para>
        /// <para>
        /// Be warned that there is a danger in using this method,
        /// as it obfuscates the object model.
        /// </para>
        /// </summary>       
        public static JExpression direct(string source)
        {
            return new JExpressionImplAnonymousInnerClass2(source);
        }

        private class JExpressionImplAnonymousInnerClass2 : JExpressionImpl
        {
            private string source;

            public JExpressionImplAnonymousInnerClass2(string source)
            {
                this.source = source;
            }

            public override void generate(JFormatter f)
            {
                f.p('(').p(source).p(')');
            }
        }
    }
}
