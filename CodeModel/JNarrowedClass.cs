//-----------------------------------------------------------------------
// <copyright file="JNarrowedClass.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace CodeModel
{
    internal class JNarrowedClass : JClass
    {
        /// <summary>
        /// A generic class with type parameters.
        /// </summary>
        internal readonly JClass basis;
        /// <summary>
        /// Arguments to those parameters.
        /// </summary>
        private readonly IList<JClass> args;

        internal JNarrowedClass(JClass basis, JClass arg)
            : this(basis, SingletonList(arg))
        {
        }

        internal JNarrowedClass(JClass basis, IList<JClass> args)
            : base(basis.owner())
        {
            this.basis = basis;
            Debug.Assert(!(basis is JNarrowedClass));
            this.args = args;
        }

        public override JClass narrow(JClass clazz)
        {
            IList<JClass> newArgs = new List<JClass>(args);
            newArgs.Add(clazz);
            return new JNarrowedClass(basis, newArgs);
        }

        public override JClass narrow(JClass[] clazz)
        {
            IList<JClass> newArgs = new List<JClass>(args);
            ((List<JClass>)newArgs).AddRange(clazz.ToList());
            return new JNarrowedClass(basis, newArgs);
        }

        public override string name()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(basis.name());
            buf.Append('<');
            bool first = true;
            foreach (JClass c in args)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    buf.Append(',');
                }
                buf.Append(c.name());
            }
            buf.Append('>');
            return buf.ToString();
        }

        public override string fullName()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(basis.fullName());
            buf.Append('<');
            bool first = true;
            foreach (JClass c in args)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    buf.Append(',');
                }
                buf.Append(c.fullName());
            }
            buf.Append('>');
            return buf.ToString();
        }

        public string binaryName()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(basis.binaryName());
            buf.Append('<');
            bool first = true;
            foreach (JClass c in args)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    buf.Append(',');
                }
                buf.Append(c.binaryName());
            }
            buf.Append('>');
            return buf.ToString();
        }

        public override void generate(JFormatter f)
        {
            f.t(basis).p('<').g(args).p(JFormatter.CLOSE_TYPE_ARGS);
        }

        internal override void printLink(JFormatter f)
        {
            basis.printLink(f);
            f.p("{@code <}");
            bool first = true;
            foreach (JClass c in args)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    f.p(',');
                }
                c.printLink(f);
            }
            f.p("{@code >}");
        }

        public override JPackage _package()
        {
            return basis._package();
        }

        public override JClass _extends()
        {
            JClass @base = basis._extends();
            if (@base == null)
            {
                return @base;
            }
            return @base.substituteParams(basis.typeParams(), args);
        }

        public override IEnumerator<JClass> _implements()
        {
            return new IteratorAnonymousInnerClass(this);
        }

        private class IteratorAnonymousInnerClass :  IEnumerator<JClass>
        {
            private readonly JNarrowedClass outerInstance;

            public IteratorAnonymousInnerClass(JNarrowedClass outerInstance)
            {
                this.outerInstance = outerInstance;
                core = outerInstance.basis._implements();
            }

            private readonly IEnumerator<JClass> core;

            public virtual void remove()
            {
                core.Dispose();
            }
            public virtual JClass next()
            {
                return core.Current;
            }
            public virtual bool hasNext()
            {
                return core.MoveNext();
            }
            public void Dispose() { }
            public void Reset() { }
            public bool MoveNext() {
                return this.MoveNext();
            }
            object System.Collections.IEnumerator.Current
            {
                get { return ((System.Collections.IEnumerator)this.core).Current; }
            }

            JClass IEnumerator<JClass>.Current
            {
                get { return ((IEnumerator<JClass>)this.core).Current; }
            }
        }

        public override JType erasure()
        {
            return basis;
        }

        public override bool isInterface
        {
            get
            {
                return basis.isInterface;
            }
        }

        public override bool isAbstract
        {
            get
            {
                return basis.isAbstract;
            }
        }

        public bool isArray
        {
            get
            {
                return false;
            }
        }


        //
        // Equality is based on value
        //

        public bool equals(object obj)
        {
            if (!(obj is JNarrowedClass))
            {
                return false;
            }
            return fullName().Equals(((JClass)obj).fullName());
        }

        public int hashCode()
        {
            return fullName().GetHashCode();
        }

        public override JClass substituteParams(JTypeVar[] variables, IList<JClass> bindings)
        {
            JClass b = basis.substituteParams(variables, bindings);
            bool different = b != basis;

            IList<JClass> clazz = new List<JClass>(args.Count);
            for (int i = 0; i < clazz.Count; i++)
            {
                JClass c = args[i].substituteParams(variables, bindings);
                clazz[i] = c;
                different |= c != args[i];
            }

            if (different)
            {
                return new JNarrowedClass(b, clazz);
            }
            else
            {
                return this;
            }
        }

        public override IList<JClass> TypeParameters
        {
            get
            {
                return args;
            }
        }

        static IList<T> SingletonList<T>(T item)
        {
            List<T> Result = new List<T>();
            Result.Add(item);
            return Result.AsReadOnly();
        }

    }
}
