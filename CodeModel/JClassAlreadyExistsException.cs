//-----------------------------------------------------------------------
// <copyright file="JClassAlreadyExistsException.cs">
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
    /// Indicates that the class is already created.
    /// </summary>
    public class JClassAlreadyExistsException : Exception
    {
        private const long serialVersionUID = 1L;

        private readonly JDefinedClass existing;

        public JClassAlreadyExistsException(JDefinedClass _existing)
        {
            this.existing = _existing;
        }

        /// <summary>
        /// Gets a reference to the existing <seealso cref="JDefinedClass"/>.
        /// 
        /// @return
        ///      This method always return non-null valid object.
        /// </summary>
        public virtual JDefinedClass existingClass
        {
            get
            {
                return existing;
            }
        }
    }
}
