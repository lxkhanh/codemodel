//-----------------------------------------------------------------------
// <copyright file="PrologCodeWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel.writer
{
    public class PrologCodeWriter : FilterCodeWriter
    {

        /// <summary>
        /// prolog comment </summary>
        private readonly string prolog;

        /// <param name="core">
        ///      This CodeWriter will be used to actually create a storage for files.
        ///      PrologCodeWriter simply decorates this underlying CodeWriter by
        ///      adding prolog comments. </param>
        /// <param name="prolog">
        ///      Strings that will be added as comments.
        ///      This string may contain newlines to produce multi-line comments.
        ///      '//' will be inserted at the beginning of each line to make it
        ///      a valid Java comment, so the caller can just pass strings like
        ///      "abc\ndef"  </param>
        public PrologCodeWriter(CodeWriter core, string prolog)
            : base(core)
        {
            this.prolog = prolog;
        }
     
        public override TextWriter openSource(JPackage pkg, string fileName)
        {
            TextWriter w = base.openSource(pkg, fileName);      

            // write prolog if this is a java source file
            if (!string.ReferenceEquals(prolog, null))
            {
                w.WriteLine("//");

                string s = prolog;
                int idx;
                while ((idx = s.IndexOf('\n')) != -1)
                {
                    w.WriteLine("// " + s.Substring(0, idx));
                    s = s.Substring(idx + 1);
                }
                w.WriteLine("//");
                w.WriteLine();
            }
            w.Flush(); // we can't close the stream for that would close the undelying stream.

            return w;
        }
    }
}
