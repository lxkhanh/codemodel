//-----------------------------------------------------------------------
// <copyright file="JBinaryFile.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel.fmt
{
    public sealed class JBinaryFile : JResourceFile
    {

        private readonly MemoryStream baos = new MemoryStream();

        public JBinaryFile(string name)
            : base(name)
        {
        }

        /// 
        /// <summary>
        /// @return
        ///      Data written to the returned output stream will be written
        ///      to the file.
        /// </summary>
        public Stream dataStore
        {
            get
            {
                return baos;
            }
        }
    
        public override void build(Stream os)
        {
            os.Write(baos.ToArray(), 0, baos.ToArray().Length);
        }
    }

}
