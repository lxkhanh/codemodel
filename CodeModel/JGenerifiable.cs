//-----------------------------------------------------------------------
// <copyright file="JGenerifiable.cs">
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
    /// Declarations that can have type variables.
    /// Something that can be made into a generic.  
    public interface JGenerifiable
    {
        /// <summary>
        /// Adds a new type variable to this declaration.
        /// </summary>
        JTypeVar generify(string name);

        /// <summary>
        /// Adds a new type variable to this declaration with a bound.
        /// </summary>
        JTypeVar generify(string name, Type bound);

        /// <summary>
        /// Adds a new type variable to this declaration with a bound.
        /// </summary>
        JTypeVar generify(string name, JClass bound);

        /// <summary>
        /// Iterates all the type parameters of this class/interface.
        /// </summary>
        JTypeVar[] typeParams();
    }

}
