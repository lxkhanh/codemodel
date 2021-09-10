//-----------------------------------------------------------------------
// <copyright file="JSerializedObject.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace CodeModel.fmt
{
    /// <summary>
    /// A simple class that takes an object and serializes it into a file
    /// in the parent package with the given name.
    /// </summary>
    public class JSerializedObject : JResourceFile
    {

        private readonly object obj;

        public JSerializedObject(string name, object obj)
            : base(name)
        {
            this.obj = obj;
        }

        /// <summary>
        /// called by JPackage to serialize the object 
        /// </summary>       
        public override void build(System.IO.Stream os)
        {
            // serialize the obj into a ByteArrayOutputStream
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(os, obj);      
        }
    }
}
