//-----------------------------------------------------------------------
// <copyright file="JMod.cs">
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
    /// Modifier constants.
    /// </summary>
    public sealed class JMod
    {
        public const int NONE = 0x000;
        public const int PUBLIC = 0x001;
        public const int PROTECTED = 0x002;
        public const int PRIVATE = 0x004;
        public const int FINAL = 0x008;
        public const int STATIC = 0x010;
        public const int ABSTRACT = 0x020;
        public const int NATIVE = 0x040;
        public const int SYNCHRONIZED = 0x080;
        public const int TRANSIENT = 0x100;
        public const int VOLATILE = 0x200;
    }
}
