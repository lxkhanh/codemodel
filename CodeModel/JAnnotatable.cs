//-----------------------------------------------------------------------
// <copyright file="JAnnotatable.cs">
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
    /// <summary>
    /// Annotatable program elements.
    /// 
    /// @author Kohsuke Kawaguchi
    /// </summary>
    public interface JAnnotatable
    {
        /// <summary>
        /// Adds an annotation to this program element. </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the program element with </param>
        JAnnotationUse annotate(JClass clazz);

        /// <summary>
        /// Adds an annotation to this program element.
        /// </summary>
        /// <param name="clazz">
        ///          The annotation class to annotate the program element with </param>
        JAnnotationUse annotate(Type clazz);

        /// <summary>
        /// Adds an annotation to this program element
        /// and returns a type-safe writer to fill in the values of such annotations.
        /// </summary>
        W annotate2<W, A>(W clazz) where W : JAnnotationWriter<A> where A: Attribute;

        /// <summary>
        /// Read-only live view of all annotations on this <seealso cref="JAnnotatable"/>
        /// 
        /// @return
        ///      Can be empty but never null.
        /// </summary>
        ICollection<JAnnotationUse> annotations();
    }
}
