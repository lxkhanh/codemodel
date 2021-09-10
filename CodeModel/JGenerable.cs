//-----------------------------------------------------------------------
// <copyright file="JGenerable.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    /**
     * Common interface for code components that can generate
     * uses of themselves.
     */

    public interface JGenerable
    {
       void generate(JFormatter f);
    }
}
