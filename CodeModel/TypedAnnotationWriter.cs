//-----------------------------------------------------------------------
// <copyright file="TypedAnnotationWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using CodeModel.DynamicProxy;

namespace CodeModel
{
    public class TypedAnnotationWriter<A, W> : JAnnotationWriter<A>
        where A : Attribute
        where W : JAnnotationWriter<A>
    {
        /// <summary>
        /// This is what we are writing to.
        /// </summary>
        private readonly JAnnotationUse use;

        /// <summary>
        /// The annotation that we are writing.
        /// </summary>
        private readonly Type annotation;

        /// <summary>
        /// The type of the writer.
        /// </summary>
        private readonly Type writerType;

        /// <summary>
        /// Keeps track of writers for array members.
        /// Lazily created.
        /// </summary>
        private IDictionary<string, JAnnotationArrayMember> arrays;

        public TypedAnnotationWriter(Type annotation, Type writer, JAnnotationUse use)
        {
            this.annotation = annotation;
            this.writerType = writer;
            this.use = use;
        }

        public virtual JAnnotationUse AnnotationUse
        {
            get
            {
                return use;
            }
        }

        public virtual Type AnnotationType
        {
            get
            {
                return annotation;
            }
        }
       
        public virtual object invoke(object proxy, MethodInfo method, object[] args)
        {

            if (method.DeclaringType == typeof(JAnnotationWriter<>))
            {
                try
                {
                    return method.Invoke(this, args);
                }
                catch (TargetInvocationException e)
                {
                    throw e;
                }
            }

            string name = method.Name;
            object arg = null;
            if (args != null && args.Length > 0)
            {
                arg = args[0];
            }

            // check how it's defined on the annotation
            MethodInfo m = typeof(A).GetMethod(name);
            Type rt = m.ReturnType;

            // array value
            if (rt.IsArray)
            {
                return addArrayValue(proxy, name, rt.GetElementType(), method.ReturnType, arg);
            }

            // sub annotation
            if (rt.IsAssignableFrom(typeof(Attribute)))
            {
                Type r = (Type)rt;
                //return (new TypedAnnotationWriter<Attribute, JAnnotationWriter<Attribute>>(r, method.ReturnType, use.annotationParam(name, r))).createProxy();
            }

            // scalar value

            if (arg is JType)
            {
                JType targ = (JType)arg;
                checkType(typeof(Type), rt);
                if (m.GetType().IsValueType)
                {
                    // check the default
                    if (targ.Equals(targ.owner().@ref((Type)m.DeclaringType)))
                    {
                        return proxy; // defaulted
                    }
                }
                use.param(name, targ);
                return proxy;
            }

            // other Java built-in types
            checkType(arg.GetType(), rt);
            if (m.DeclaringType != null && m.DeclaringType.Equals(arg))
            {
                // defaulted. no need to write out.
                return proxy;
            }

            if (arg is string)
            {
                use.param(name, (string)arg);
                return proxy;
            }
            if (arg is bool?)
            {
                use.param(name, (bool)arg);
                return proxy;
            }
            if (arg is int?)
            {
                use.param(name, (int)arg);
                return proxy;
            }
            if (arg is Type)
            {
                use.param(name, (Type)arg);
                return proxy;
            }
            if (arg is Enum)
            {
                use.param(name, (Enum)arg);
                return proxy;
            }

            throw new System.ArgumentException("Unable to handle this method call " + method.ToString());
        }
      
        private object addArrayValue(object proxy, string name, Type itemType, Type expectedReturnType, object arg)
        {
            if (arrays == null)
            {
                arrays = new Dictionary<string, JAnnotationArrayMember>();
            }
            JAnnotationArrayMember m = null;
            if (arrays != null && arrays.ContainsKey(name)) { 
                m = arrays[name];
            }          
            if (m == null)
            {
                m = use.paramArray(name);
                arrays[name] = m;
            }

            // sub annotation
            if (itemType.IsAssignableFrom(typeof(Attribute)))
            {
                Type r = (Type)itemType;
                if (!expectedReturnType.IsAssignableFrom(typeof(JAnnotationWriter<A>)))
                {
                    throw new System.ArgumentException("Unexpected return type " + expectedReturnType);
                }
                //return (new TypedAnnotationWriter<A, W>(r, expectedReturnType, m.annotate(r))).createProxy();
            }

            // primitive
            if (arg is JType)
            {
                checkType(typeof(Type), itemType);
                m.param((JType)arg);
                return proxy;
            }
            checkType(arg.GetType(), itemType);
            if (arg is string)
            {
                m.param((string)arg);
                return proxy;
            }
            if (arg is bool?)
            {
                m.param((bool)arg);
                return proxy;
            }
            if (arg is int?)
            {
                m.param((int)arg);
                return proxy;
            }
            if (arg is Type)
            {
                m.param((Type)arg);
                return proxy;
            }
            // TODO: enum constant. how should we handle it?

            throw new System.ArgumentException("Unable to handle this method call ");
        }


        /// <summary>
        /// Check if the type of the argument matches our expectation.
        /// If not, report an error.
        /// </summary>
        private void checkType(Type actual, Type expected)
        {
            if (expected == actual || expected.IsAssignableFrom(actual))
            {
                return; // no problem
            }
            Type value  ;
            JCodeModel.boxToPrimitive.TryGetValue(actual,out value);
            if (expected == value)
            {
                return; // no problem
            }

            throw new System.ArgumentException("Expected " + expected + " but found " + actual);
        }

        /// <summary>
        /// Creates a proxy and returns it.
        /// </summary>

        private W createProxy()
        {
            return (W)SecurityProxy.NewInstance(writerType);
        }

        /// <summary>
        /// Creates a new typed annotation writer.
        /// </summary>

        internal static W create<A, W>(W w, JAnnotatable annotatable)
            where W : JAnnotationWriter<A>
            where A : Attribute
        {
            A a = findAnnotationType<A>(w as A);
            return (W)(new TypedAnnotationWriter<A, W>(typeof(A), typeof(W), annotatable.annotate(typeof(A)))).createProxy();
        }

        private static A findAnnotationType<A>(A clazz) where A : Attribute
        {
            foreach (Type t in clazz.GetType().GetInterfaces())
            {
                if (t.IsGenericType)
                {
                    if (t == typeof(JAnnotationWriter<A>))
                    {
                        return t.GetElementType() as A;
                    }
                }
                if (t is A)
                {
                    // recursive search
                    A r = findAnnotationType(t as A);
                    if (r != null)
                    {
                        return r as A;
                    }
                }
            }
            return null;
        }

    }
}
