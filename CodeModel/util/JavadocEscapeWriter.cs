//-----------------------------------------------------------------------
// <copyright file="JavadocEscapeWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel.util
{
    /// <summary>
    /// <seealso cref="Writer"/> that escapes characters that are unsafe
    /// as Javadoc comments.
    /// 
    /// Such characters include '&lt;' and '&amp;'.
    /// 
    /// <para>
    /// Note that this class doesn't escape other Unicode characters
    /// that are typically unsafe. For example, &#x611B; (A kanji
    /// that means "love") can be considered as unsafe because
    /// javac with English Windows cannot accept this character in the
    /// source code.
    /// 
    /// </para>
    /// <para>
    /// If the application needs to escape such characters as well, then
    /// they are on their own.
    /// </para>
    /// </summary>
    public class JavadocEscapeWriter : StringWriter
    {
        private StreamWriter @out;
        public JavadocEscapeWriter(StreamWriter next) : base()
        {
            @out = next;
        }

        public override void Write(int ch)
        {
            if (ch == '<')
            {
                @out.Write("&lt;");
            }
            else
            {
                if (ch == '&')
                {
                    @out.Write("&amp;");
                }
                else
                {
                    @out.Write(ch);
                }
            }
        }

        public override void Write(char[] buf, int off, int len)
        {
            for (int i = 0; i < len; i++)
            {
                Write(buf[off + i]);
            }
        }


        public override void Write(char[] buf)
        {
            Write(buf, 0, buf.Length);
        }


        public virtual void Write(string buf, int off, int len)
        {
            Write(buf.ToCharArray(), off, len);
        }


        public override void Write(string buf)
        {
            Write(buf.ToCharArray(), 0, buf.Length);
        }

    }
}
