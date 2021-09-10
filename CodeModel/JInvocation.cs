//-----------------------------------------------------------------------
// <copyright file="JInvocation.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public sealed class JInvocation : JExpressionImpl, JStatement
    {

        /// <summary>
        /// Object expression upon which this method will be invoked, or null if
        /// this is a constructor invocation
        /// </summary>
        private JGenerable @object;

        /// <summary>
        /// Name of the method to be invoked.
        /// Either this field is set, or <seealso cref="#method"/>, or <seealso cref="#type"/> (in which case it's a
        /// constructor invocation.)
        /// This allows <seealso cref="JMethod#name(String) the name of the method to be changed later"/>.
        /// </summary>
        private string name;

        private JMethod method;

        private bool isConstructor = false;

        /// <summary>
        /// List of argument expressions for this method invocation
        /// </summary>
        private IList<JExpression> args = new List<JExpression>();

        /// <summary>
        /// If isConstructor==true, this field keeps the type to be created.
        /// </summary>
        private JType type = null;

        /// <summary>
        /// Invokes a method on an object.
        /// </summary>
        /// <param name="object">
        ///        JExpression for the object upon which
        ///        the named method will be invoked,
        ///        or null if none
        /// </param>
        /// <param name="name">
        ///        Name of method to invoke </param>
        internal JInvocation(JExpression @object, string name)
            : this((JGenerable)@object, name)
        {
        }

        internal JInvocation(JExpression @object, JMethod method)
            : this((JGenerable)@object, method)
        {
        }

        /// <summary>
        /// Invokes a static method on a class.
        /// </summary>
        internal JInvocation(JClass type, string name)
            : this((JGenerable)type, name)
        {
        }

        internal JInvocation(JClass type, JMethod method)
            : this((JGenerable)type, method)
        {
        }

        private JInvocation(JGenerable @object, string name)
        {
            this.@object = @object;
            if (name.IndexOf('.') >= 0)
            {
                throw new System.ArgumentException("method name contains '.': " + name);
            }
            this.name = name;
        }

        private JInvocation(JGenerable @object, JMethod method)
        {
            this.@object = @object;
            this.method = method;
        }

        /// <summary>
        /// Invokes a constructor of an object (i.e., creates
        /// a new object.)
        /// </summary>
        /// <param name="c">
        ///      Type of the object to be created. If this type is
        ///      an array type, added arguments are treated as array
        ///      initializer. Thus you can create an expression like
        ///      <code>new int[]{1,2,3,4,5}</code>. </param>
        internal JInvocation(JType c)
        {
            this.isConstructor = true;
            this.type = c;
        }

        /// <summary>
        ///  Add an expression to this invocation's argument list
        /// </summary>
        /// <param name="arg">
        ///        Argument to add to argument list </param>
        public JInvocation arg(JExpression arg)
        {
            if (arg == null)
            {
                throw new System.ArgumentException();
            }
            args.Add(arg);
            return this;
        }

        /// <summary>
        /// Adds a literal argument.
        /// 
        /// Short for {@code arg(JExpr.lit(v))}
        /// </summary>
        public JInvocation arg(string v)
        {
            return arg(JExpr.lit(v));
        }

        /// <summary>
        /// Returns all arguments of the invocation.
        /// @return
        ///      If there's no arguments, an empty array will be returned.
        /// </summary>
        public JExpression[] listArgs()
        {
            return args.ToArray();
        }

        public override void generate(JFormatter f)
        {
            if (isConstructor && type.isArray())
            {
                // [RESULT] new T[]{arg1,arg2,arg3,...};
                f.p("new").g(type).p('{');
            }
            else
            {
                if (isConstructor)
                {
                    f.p("new").g(type).p('(');
                }
                else
                {
                    string name = this.name;
                    if (name == null)
                    {
                        name = this.method.name();
                    }

                    if (@object != null)
                    {
                        f.g(@object).p('.').p(name).p('(');
                    }
                    else
                    {
                        f.id(name).p('(');
                    }
                }
            }

            f.g(args);

            if (isConstructor && type.isArray())
            {
                f.p('}');
            }
            else
            {
                f.p(')');
            }

            if (type is JDefinedClass && ((JDefinedClass)type).isAnonymous)
            {
                ((JAnonymousClass)type).declareBody(f);
            }
        }

        public void state(JFormatter f)
        {
            f.g(this).p(';').nl();
        }

    }
}
