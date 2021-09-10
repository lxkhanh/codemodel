//-----------------------------------------------------------------------
// <copyright file="OutputStreamCodeWriter.cs">
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
    public class OutputStreamCodeWriter : CodeWriter
    {
        private readonly StreamWriter @out;

        /// <param name="os">
        ///            This stream will be closed at the end of the code generation. </param>
        public OutputStreamCodeWriter(System.IO.Stream os, string encoding)
        {
            try
            {
                this.@out = new StreamWriter(os, Encoding.GetEncoding(encoding));
            }
            catch (Exception ueex)
            {
                throw new System.ArgumentException(ueex.Message);
            }
            this.encoding = encoding;
        }
       
        public override Stream openBinary(JPackage pkg, string fileName)
        {
            return new FilterOutputStreamAnonymousInnerClass(this, @out, fileName);
        }

        private class FilterOutputStreamAnonymousInnerClass : FileStream
        {
            private readonly OutputStreamCodeWriter outerInstance;

            public FilterOutputStreamAnonymousInnerClass(OutputStreamCodeWriter outerInstance, StreamWriter @out, string fileName) : base(fileName, FileMode.OpenOrCreate)
            {
                this.outerInstance = outerInstance;
            }

            public void close()
            {
                // don't let this stream close
            }
        }

        public override void close()
        {
            @out.Close();
        }
    }
}
