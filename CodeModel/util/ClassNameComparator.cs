//-----------------------------------------------------------------------
// <copyright file="ClassNameComparator.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel.util
{
    /// <summary>
    /// Comparator object that sorts <seealso cref="JClass"/>es in the order
    /// of their names. 
    /// </summary>
    public class ClassNameComparator : IComparer<JClass>
    {
        private ClassNameComparator()
        {
        }

        public virtual int Compare(JClass l, JClass r)
        {
            return l.fullName().CompareTo(r.fullName());
        }

        public static readonly IComparer<JClass> theInstance = new ClassNameComparator();
    }
}
