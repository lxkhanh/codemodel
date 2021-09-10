//-----------------------------------------------------------------------
// <copyright file="JAnnotationValue.cs">
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
    /// Things that can be values of an annotation element.
    /// 
    /// @author
    ///     Bhakti Mehta (bhakti.mehta@sun.com)
    /// </summary>
    public abstract class JAnnotationValue : JGenerable
    {
        public abstract void generate(JFormatter f);
    }
}
