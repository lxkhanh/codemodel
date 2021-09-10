//-----------------------------------------------------------------------
// <copyright file="JAnnotationUse.cs">
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
    /// Represents an annotation on a program element.
    /// 
    /// TODO
    ///    How to add enums to the annotations
    /// @author
    ///     Bhakti Mehta (bhakti.mehta@sun.com)
    /// </summary>
    public sealed class JAnnotationUse : JAnnotationValue
    {

        /// <summary>
        /// The <seealso cref="Annotation"/> class
        /// </summary>
        private readonly JClass clazz;

        /// <summary>
        /// Map of member values.
        /// </summary>
        private IDictionary<string, JAnnotationValue> memberValues;

        internal JAnnotationUse(JClass clazz)
        {
            this.clazz = clazz;
        }

        public JClass AnnotationClass
        {
            get
            {
                return clazz;
            }
        }

        public IDictionary<string, JAnnotationValue> AnnotationMembers
        {
            get
            {
                if (memberValues == null) {
                    return new Dictionary<string, JAnnotationValue>();
                }
                return memberValues;
            }
        }

        private JCodeModel owner()
        {
            return clazz.owner();
        }

        private void addValue(string name, JAnnotationValue annotationValue)
        {
            // Use ordered map to keep the code generation the same on any JVM.
            // Lazily created.
            if (memberValues == null)
            {
                memberValues = new Dictionary<string, JAnnotationValue>();
            }
            memberValues.Add(name, annotationValue);
        }

        /// <summary>
        /// Adds a member value pair to this annotation
        /// </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The boolean value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, bool value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The byte member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, sbyte value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The char member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, char value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The double member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, double value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The float member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, float value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The long member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, long value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The short member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, short value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The int member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, int value)
        {
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The String member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, string value)
        {
            //Escape string values with quotes so that they can
            //be generated accordingly
            addValue(name, new JAnnotationStringValue(JExpr.lit(value)));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation
        /// For adding class values as param </summary>
        /// <seealso cref= #param(String, Class) </seealso>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The annotation class which is member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse annotationParam(string name, Type value)
        {
            JAnnotationUse annotationUse = new JAnnotationUse(owner().@ref(value));
            addValue(name, annotationUse);
            return annotationUse;
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The enum class which is member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, Enum value)
        {
            addValue(name, new JAnnotationValueAnonymousInnerClass(this, value));
            return this;
        }

        private class JAnnotationValueAnonymousInnerClass : JAnnotationValue
        {
            private readonly JAnnotationUse outerInstance;

            private Enum value;

            public JAnnotationValueAnonymousInnerClass(JAnnotationUse outerInstance, Enum value)
            {
                this.outerInstance = outerInstance;
                this.value = value;
            }

            public override void generate(JFormatter f)
            {
                f.t(outerInstance.owner().@ref(value.GetType())).p('.').p(value.ToString());
            }
        }

        /// <summary>
        /// Adds a member value pair to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The JEnumConstant which is member value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, JEnumConstant value)
        {
            addValue(name, new JAnnotationStringValue(value));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation
        ///  This can be used for e.g to specify
        /// <pre>
        ///        &#64;XmlCollectionItem(type=Integer.class);
        /// <pre>
        /// For adding a value of Class<? extends Annotation>
        /// @link
        /// #annotationParam(java.lang.String, java.lang.Class<? extends java.lang.annotation.Annotation>) </summary>
        /// <param name="name">
        ///        The simple name for this annotation param
        /// </param>
        /// <param name="value">
        ///        The class type of the param
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        /// 
        /// 
        ///  </param>

        public JAnnotationUse param(string name, Type value)
        {
            addValue(name, new JAnnotationStringValue(new JExpressionImplAnonymousInnerClass(this, value)));
            return this;
        }

        private class JExpressionImplAnonymousInnerClass : JExpressionImpl
        {
            private readonly JAnnotationUse outerInstance;

            private Type value;

            public JExpressionImplAnonymousInnerClass(JAnnotationUse outerInstance, Type value)
            {
                this.outerInstance = outerInstance;
                this.value = value;
            }

            public override void generate(JFormatter f)
            {              
                f.p(value.FullName.Replace('$', '.'));
                f.p(".class");
            }
        }

        /// <summary>
        /// Adds a member value pair to this annotation based on the
        /// type represented by the given JType
        /// </summary>
        /// <param name="name"> The simple name for this annotation param </param>
        /// <param name="type"> the JType representing the actual type </param>
        /// <returns> The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods. </returns>
        public JAnnotationUse param(string name, JType type)
        {
            JClass c = type.boxify();
            addValue(name, new JAnnotationStringValue(c.dotclass()));
            return this;
        }

        /// <summary>
        /// Adds a member value pair to this annotation. </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// </param>
        /// <param name="value">
        ///        The JExpression which provides the contant value for this annotation
        /// @return
        ///         The JAnnotationUse. More member value pairs can
        ///         be added to it using the same or the overloaded methods.
        ///  </param>
        public JAnnotationUse param(string name, JExpression value)
        {
            addValue(name, new JAnnotationStringValue(value));
            return this;
        }

        /// <summary>
        /// Adds a member value pair which is of type array to this annotation </summary>
        /// <param name="name">
        ///        The simple name for this annotation
        /// 
        /// @return
        ///         The JAnnotationArrayMember. For adding array values </param>
        ///         <seealso cref= JAnnotationArrayMember
        ///  </seealso>
        public JAnnotationArrayMember paramArray(string name)
        {
            JAnnotationArrayMember arrayMember = new JAnnotationArrayMember(owner());
            addValue(name, arrayMember);
            return arrayMember;
        }


        //    /**
        //     * This can be used to add annotations inside annotations
        //     * for e.g  &#64;XmlCollection(values= &#64;XmlCollectionItem(type=Foo.class))
        //     * @param className
        //     *         The classname of the annotation to be included
        //     * @return
        //     *         The JAnnotationUse that can be used as a member within this JAnnotationUse
        //     * @deprecated
        //     *      use {@link JAnnotationArrayMember#annotate}
        //     */
        //    public JAnnotationUse annotate(String className) {
        //        JAnnotationUse annotationUse = new JAnnotationUse(owner().ref(className));
        //        return annotationUse;
        //    }

        /// <summary>
        /// This can be used to add annotations inside annotations
        /// for e.g  &#64;XmlCollection(values= &#64;XmlCollectionItem(type=Foo.class)) </summary>
        /// <param name="clazz">
        ///         The annotation class to be included
        /// @return
        ///     The JAnnotationUse that can be used as a member within this JAnnotationUse
        /// @deprecated
        ///      use <seealso cref="JAnnotationArrayMember#annotate"/> </param>
        public JAnnotationUse annotate(Type clazz)
        {
            JAnnotationUse annotationUse = new JAnnotationUse(owner().@ref(clazz));
            return annotationUse;
        }

        public override void generate(JFormatter f)
        {
            f.p('@').g(clazz);
            if (memberValues != null)
            {
                f.p('(');
                bool first = true;

                if (isOptimizable)
                {
                    // short form
                    if (memberValues.ContainsKey("value")) {
                        f.g(memberValues["value"]);
                    }                   
                }
                else
                {
                    foreach (KeyValuePair<string, JAnnotationValue> mapEntry in memberValues)
                    {
                        if (!first)
                        {
                            f.p(',');
                        }
                        f.p(mapEntry.Key).p('=').g(mapEntry.Value);
                        first = false;
                    }
                }
                f.p(')');
            }
        }

        private bool isOptimizable
        {
            get
            {
                return memberValues.Count == 1 && memberValues.ContainsKey("value");
            }
        }
    }
}
