//-----------------------------------------------------------------------
// <copyright file="UnicodeEscapeWriter.cs">
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
    public class UnicodeEscapeWriter : StreamWriter
    {
        private StreamWriter @out;
        public UnicodeEscapeWriter(StreamWriter next):base(next.BaseStream)
        {
            @out = next;
        }   

        public void write(int ch)
		{
			if (!requireEscaping(ch))
			{
                @out.Write(ch);
			}
			else
			{
				// need to escape
				write("\\u");
                string s = ch.ToString("X") ;
				for (int i = s.Length; i < 4; i++)
				{
					write('0');
				}
				write(s);
			}
		}

        /// <summary>
        /// Can be overrided. Return true if the character
        /// needs to be escaped. 
        /// </summary>
        protected internal virtual bool requireEscaping(int ch)
        {
            if (ch >= 128)
            {
                return true;
            }

            // control characters
            if (ch < 0x20 && " \t\r\n".IndexOf(Convert.ToChar(ch)) == -1)
            {
                return true;
            }

            return false;
        }
        
        public void write(char[] buf, int off, int len)
        {
            for (int i = 0; i < len; i++)
            {
                @out.Write(buf[off + i]);
            }
        }
       
        public void write(char[] buf)
        {
            @out.Write(buf, 0, buf.Length);
        }

        public void write(string buf, int off, int len)
        {
            @out.Write(buf.ToCharArray(), off, len);
        }
     
        public void write(string buf)
        {
            @out.Write(buf.ToCharArray(), 0, buf.Length);
        }

    }
}
