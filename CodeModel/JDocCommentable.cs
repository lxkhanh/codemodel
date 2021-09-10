//-----------------------------------------------------------------------
// <copyright file="JDocCommentable.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    public interface JDocCommentable
    {
        /**
         * @return the JavaDoc of the Element
         */
        JDocComment javadoc();
    }

}
