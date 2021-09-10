//-----------------------------------------------------------------------
// <copyright file="JAnnotationArrayMember.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CodeModel
{
    public sealed class JAnnotationArrayMember : JAnnotationValue, JAnnotatable
    {
        private readonly IList<JAnnotationValue> values = new List<JAnnotationValue>();
        private readonly JCodeModel owner;

        internal JAnnotationArrayMember(JCodeModel owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a string value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(string value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a boolean value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(bool value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a byte value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(sbyte value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a char value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(char value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a double value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(double value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a long value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(long value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a short value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(short value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds an int value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(int value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a float value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
        public JAnnotationArrayMember param(float value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(JExpr.lit(value));
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds a enum array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a enum value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
       
        public JAnnotationArrayMember param<T1>(Enum value)
        {
            JAnnotationValue annotationValue = new JAnnotationValueAnonymousInnerClass(this, value);
            values.Add(annotationValue);
            return this;
        }

        private class JAnnotationValueAnonymousInnerClass : JAnnotationValue
        {
            private readonly JAnnotationArrayMember outerInstance;

            private Enum value;

            public JAnnotationValueAnonymousInnerClass(JAnnotationArrayMember outerInstance, Enum value)
            {
                this.outerInstance = outerInstance;
                this.value = value;
            }

            public override void generate(JFormatter f)
            {
                f.t(outerInstance.owner.@ref(value.GetType().DeclaringType)).p('.').p(value.ToString());
            }
        }

        /// <summary>
        /// Adds a enum array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a enum value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>      
        public JAnnotationArrayMember param(JEnumConstant value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(value);
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds an expression array member to this annotation
        /// </summary>
        /// <param name="value"> Adds an expression value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
       
        public JAnnotationArrayMember param(JExpression value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(value);
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds a class array member to this annotation
        /// </summary>
        /// <param name="value"> Adds a class value to the array member </param>
        /// <returns> The JAnnotationArrayMember. More elements can be added by calling
        ///         the same method multiple times </returns>
       
        public JAnnotationArrayMember param(Type value)
        {
            JAnnotationValue annotationValue = new JAnnotationStringValue(new JExpressionImplAnonymousInnerClass(this, value));
            values.Add(annotationValue);
            return this;
        }

        private class JExpressionImplAnonymousInnerClass : JExpressionImpl
        {
            private readonly JAnnotationArrayMember outerInstance;

            private Type value;

            public JExpressionImplAnonymousInnerClass(JAnnotationArrayMember outerInstance, Type value)
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

        public JAnnotationArrayMember param(JType type)
        {
            JClass clazz = type.boxify();
            JAnnotationValue annotationValue = new JAnnotationStringValue(clazz.dotclass());
            values.Add(annotationValue);
            return this;
        }

        /// <summary>
        /// Adds a new annotation to the array.
        /// </summary>
        public JAnnotationUse annotate(Type clazz)
        {
            return annotate(owner.@ref(clazz));
        }

        /// <summary>
        /// Adds a new annotation to the array.
        /// </summary>
        public JAnnotationUse annotate(JClass clazz)
        {
            JAnnotationUse a = new JAnnotationUse(clazz);
            values.Add(a);
            return a;
        }

        public W annotate2<W, A>(W clazz) where W : JAnnotationWriter<A> where A : Attribute
        {
            return TypedAnnotationWriter<A, W>.create<A, W>(clazz, this);
        }

        /// <summary>
        /// <seealso cref="JAnnotatable#annotations()"/>
        /// </summary>
        public ICollection<JAnnotationUse> annotations()
        {
            // this invocation is invalid if the caller isn't adding annotations into an array
            // so this potentially type-unsafe conversion would be justified.

            return new Collection<JAnnotationValue>(values) as ICollection<JAnnotationUse>;
        }

        /// <summary>
        /// Adds an annotation member to this annotation  array
        /// This can be used for e.g &#64;XmlCollection(values= &#64;XmlCollectionItem(type=Foo.class)) </summary>
        /// <param name="value">
        ///        Adds a annotation  to the array member
        /// @return
        ///        The JAnnotationArrayMember. More elements can be added by calling
        ///        the same method multiple times
        /// 
        /// @deprecated
        ///      use <seealso cref="#annotate"/> </param>
        public JAnnotationArrayMember param(JAnnotationUse value)
        {
            values.Add(value);
            return this;
        }

        public override void generate(JFormatter f)
        {
            f.p('{').nl().i();

            bool first = true;
            foreach (JAnnotationValue aValue in values)
            {
                if (!first)
                {
                    f.p(',').nl();
                }
                f.g(aValue);
                first = false;
            }
            f.nl().o().p('}');
        }
    }
}
