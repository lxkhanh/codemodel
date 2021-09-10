//-----------------------------------------------------------------------
// <copyright file="JTextFile.cs">
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
    /// <summary>
    /// Simple text file.
    /// </summary>
    public class JTextFile : JResourceFile
    {
        public JTextFile(string name)
            : base(name)
        {
        }

        private string contents = null;

        public virtual string Contents
        {
            set
            {
                this.contents = value;
            }
        }

        public override void build(System.IO.Stream @out)
        {
            StreamWriter w = new System.IO.StreamWriter(@out);
            w.Write(contents);
            w.Close();
        }
    }
}
