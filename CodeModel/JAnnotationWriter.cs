//-----------------------------------------------------------------------
// <copyright file="JAnnotationWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public interface JAnnotationWriter<A> where A : Attribute
    {
        /// <summary>
        /// Gets the underlying annotation use object to which we are writing.
        /// </summary>
        JAnnotationUse AnnotationUse { get; }

        /// <summary>
        /// The type of the annotation that this writer is writing.
        /// </summary>
        Type AnnotationType { get; }
    }
}
