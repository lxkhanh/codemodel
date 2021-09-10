//-----------------------------------------------------------------------
// <copyright file="CodeWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeModel.util;

namespace CodeModel
{
    public abstract class CodeWriter
	{

		/// <summary>
		/// Encoding to be used by the writer. Null means platform specific encoding.
		/// 
		/// @since 2.5
		/// </summary>
		protected internal string encoding = null;

		/// <summary>
		/// Called by CodeModel to store the specified file.
		/// The callee must allocate a storage to store the specified file.
		/// 
		/// <p>
		/// The returned stream will be closed before the next file is
		/// stored. So the callee can assume that only one OutputStream
		/// is active at any given time.
		/// </summary>
		/// <param name="pkg">
		///      The package of the file to be written. </param>
		/// <param name="fileName">
		///      File name without the path. Something like
		///      "Foo.java" or "Bar.properties" </param>
        public abstract Stream openBinary(JPackage pkg, string fileName);

		/// <summary>
		/// Called by CodeModel to store the specified file.
		/// The callee must allocate a storage to store the specified file.
		/// 
		/// <p>
		/// The returned stream will be closed before the next file is
		/// stored. So the callee can assume that only one OutputStream
		/// is active at any given time.
		/// </summary>
		/// <param name="pkg">
		///      The package of the file to be written. </param>
		/// <param name="fileName">
		///      File name without the path. Something like
		///      "Foo.java" or "Bar.properties" </param>

        public virtual TextWriter openSource(JPackage pkg, string fileName)
        {
            System.IO.StreamWriter bw = !string.ReferenceEquals(encoding, null) ? new System.IO.StreamWriter(openBinary(pkg, fileName), Encoding.GetEncoding(encoding)) : new System.IO.StreamWriter(openBinary(pkg, fileName));
            // create writer
            try
            {
                return new UnicodeEscapeWriterAnonymousInnerClass(this, bw);
            }
            catch (Exception ex)
            {
                return new UnicodeEscapeWriter(bw);
            }
        }

        private class UnicodeEscapeWriterAnonymousInnerClass : UnicodeEscapeWriter
        {
            private readonly CodeWriter outerInstance;

            private System.IO.StreamWriter bw;

            public UnicodeEscapeWriterAnonymousInnerClass(CodeWriter outerInstance, System.IO.StreamWriter bw)
                : base(bw)
            {
                this.outerInstance = outerInstance;
                this.bw = bw;
                encoder = EncoderFactory.createEncoder(bw.Encoding.ToString().Substring(bw.Encoding.ToString().LastIndexOf('.') + 1));
            }
           
            private readonly Encoder encoder;
            protected internal override bool requireEscaping(int ch)
            {
                // control characters
                if (ch < 0x20 && " \t\r\n".IndexOf(Convert.ToChar(ch)) == -1)
                {
                    return true;
                }
                // check ASCII chars, for better performance
                if (ch < 0x80)
                {
                    return false;
                }                            
                return false;
            }
        }

		/// <summary>
		/// Called by CodeModel at the end of the process.
		/// </summary>
		public abstract void close();
	}
}
