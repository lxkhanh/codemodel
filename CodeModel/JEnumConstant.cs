//-----------------------------------------------------------------------
// <copyright file="JEnumConstant.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JEnumConstant : JExpressionImpl, JDeclaration, JAnnotatable, JDocCommentable
    {

        /// <summary>
        /// The constant.
        /// </summary>
        private readonly string name;
        /// <summary>
        /// The enum class.
        /// </summary>
        private readonly JDefinedClass type;
        /// <summary>
        /// javadoc comments, if any.
        /// </summary>
        private JDocComment jdoc = null;

        /// <summary>
        /// Annotations on this variable. Lazily created.
        /// </summary>
        
        private IList<JAnnotationUse> _annotations = null;


        /// <summary>
        /// List of the constructor argument expressions.
        /// Lazily constructed.
        /// </summary>
        private IList<JExpression> args = null;

        internal JEnumConstant(JDefinedClass type, string name)
        {
            this.name = name;
            this.type = type;
        }

        /// <summary>
        ///  Add an expression to this constructor's argument list
        /// </summary>
        /// <param name="arg">
        ///        Argument to add to argument list </param>
        public JEnumConstant arg(JExpression arg)
        {
            if (arg == null)
            {
                throw new System.ArgumentException();
            }
            if (args == null)
            {
                args = new List<JExpression>();
            }
            args.Add(arg);
            return this;
        }

        /// <summary>
        /// Returns the name of this constant.
        /// </summary>
        /// <returns> never null. </returns>
        public string Name
        {
            get
            {
                return this.type.fullName() + "." + this.name;
            }
        }

        /// <summary>
        /// Creates, if necessary, and returns the enum constant javadoc.
        /// </summary>
        /// <returns> JDocComment containing javadocs for this constant. </returns>
        public JDocComment javadoc()
        {
            if (jdoc == null)
            {
                jdoc = new JDocComment(type.owner());
            }
            return jdoc;
        }

        /// <summary>
        /// Adds an annotation to this variable. </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the field with </param>
        public JAnnotationUse annotate(JClass clazz)
        {
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            JAnnotationUse a = new JAnnotationUse(clazz);
            _annotations.Add(a);
            return a;
        }

        /// <summary>
        /// Adds an annotation to this variable.
        /// </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the field with </param>
        public JAnnotationUse annotate(Type clazz)
        {
            return annotate(type.owner().@ref(clazz));
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
            if (_annotations == null)
            {
                _annotations = new List<JAnnotationUse>();
            }
            return _annotations;
        }

        public void declare(JFormatter f)
        {
            if (jdoc != null)
            {
                f.nl().g((JGenerable)jdoc);
            }
            if (_annotations != null)
            {
                for (int i = 0; i < _annotations.Count; i++)
                {
                    f.g(_annotations[i]).nl();
                }
            }
            f.id(name);
            if (args != null)
            {
                f.p('(').g(args).p(')');
            }
        }

        public override void generate(JFormatter f)
        {
            f.t(type).p('.').p(name);
        }
    }
}
