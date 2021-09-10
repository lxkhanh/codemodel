//-----------------------------------------------------------------------
// <copyright file="JCommentPart.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CodeModel
{
    public class JCommentPart : List<object>
    {
        private const long serialVersionUID = 1L;

        /// <summary>
        /// Appends a new value.
        ///    
        /// If the value is <seealso cref="JType"/> it will be printed as a @link tag.
        /// Otherwise it will be converted to String via <seealso cref="Object#toString()"/>.
        /// </summary>
        public virtual JCommentPart append(object o)
        {
            add(o);
            return this;
        }

        public virtual bool add(object o)
        {
            flattenAppend(o);
            return true;
        }

        private void flattenAppend(object value)
		{
			if (value == null)
			{
				return;
			}
			if (value is object[])
			{
				foreach (object o in (object[])value)
				{
					flattenAppend(o);
				}
			}
			else if (value is ICollection)
			{
				foreach (object o in (ICollection)value)
				{
					flattenAppend(o);
				}
			}
			else
			{
				base.Add(value);
			}
		}

        /// <summary>
        /// Writes this part into the formatter by using the specified indentation.
        /// </summary>
        public void format(JFormatter f, string indent)
        {
            if (!f.isPrinting())
            {
                // quickly pass the types to JFormatter, as that's all we care.
                // we don't need to worry about the exact formatting of text.
                foreach (object o in this)
                {
                    if (o is JClass)
                    {
                        f.g((JClass)o);
                    }
                }
                return;
            }

            if (base.Count != 0)
            {
                f.p(indent);
            }

            IEnumerator<object> itr = base.GetEnumerator();
            while (itr.MoveNext())
            {
                object o = itr.Current;

                if (o is string)
                {
                    int idx;
                    string s = (string)o;
                    while ((idx = s.IndexOf('\n')) != -1)
                    {
                        string line = s.Substring(0, idx);
                        if (line.Length > 0)
                        {
                            f.p(escape(line));
                        }
                        s = s.Substring(idx + 1);
                        f.nl().p(indent);
                    }
                    if (s.Length != 0)
                    {
                        f.p(escape(s));
                    }
                }
                else
                {
                    if (o is JClass)
                    {
                        // TODO: this doesn't print the parameterized type properly
                        ((JClass)o).printLink(f);
                    }
                    else
                    {
                        if (o is JType)
                        {
                            f.g((JType)o);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }

            if (base.Count!=0)
            {
                f.nl();
            }
        }

        /// <summary>
        /// Escapes the appearance of the comment terminator.
        /// </summary>
        private string escape(string s)
        {
            while (true)
            {
                int idx = s.IndexOf("*/");
                if (idx < 0)
                {
                    return s;
                }

                s = s.Substring(0, idx + 1) + "<!---->" + s.Substring(idx + 1);
            }
        }
    }
}
