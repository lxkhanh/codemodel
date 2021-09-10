//-----------------------------------------------------------------------
// <copyright file="JResourceFile.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel
{
    /// <summary>
    /// Represents a resource file in the application-specific file format.
    /// </summary>
    public abstract class JResourceFile
    {

        private readonly string _name;

        protected internal JResourceFile(string name)
        {
            this._name = name;
        }

        /// <summary>
        /// Gets the name of this property file
        /// </summary>
        public virtual string name()
        {
            return _name;
        }

        /// <summary>
        /// Returns true if this file should be generated into the directory
        /// that the resource files go into.
        /// 
        /// <p>
        /// Returns false if this file should be generated into the directory
        /// where other source files go.
        /// </summary>
        public virtual bool isResource
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// called by JPackage to produce the file image.
        /// </summary>
        public abstract void build(Stream os);
    }
}
