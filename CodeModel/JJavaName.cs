//-----------------------------------------------------------------------
// <copyright file="JJavaName.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace CodeModel
{
    /// <summary>
    /// Utility methods that convert arbitrary strings into Java identifiers.
    /// </summary>
    public class JJavaName
    {
        /// <summary>
        /// Checks if a given string is usable as a Java identifier.
        /// </summary>
        public static bool isJavaIdentifier(string s)
        {
            if (s.Length == 0)
            {
                return false;
            }
            if (reservedKeywords.Contains(s))
            {
                return false;
            }

            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            if (!provider.IsValidIdentifier(Convert.ToString(s[0]))) {
                return false;
            }

            for (int i = 1; i < s.Length; i++)
            {
                if (!provider.IsValidIdentifier(Convert.ToString(s[i])))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the given string is a valid fully qualified name.
        /// </summary>
        public static bool isFullyQualifiedClassName(string s)
        {
            return isJavaPackageName(s);
        }

        /// <summary>
        /// Checks if the given string is a valid Java package name.
        /// </summary>
        public static bool isJavaPackageName(string s)
        {
            while (s.Length != 0)
            {
                int idx = s.IndexOf('.');
                if (idx == -1)
                {
                    idx = s.Length;
                }
                if (!isJavaIdentifier(s.Substring(0, idx)))
                {
                    return false;
                }

                s = s.Substring(idx);
                if (s.Length != 0) 
                {
                    s = s.Substring(1); // remove '.'
                }
            }
            return true;
        }

        /// <summary>
        /// <b>Experimental API:</b> converts an English word into a plural form.
        /// </summary>
        /// <param name="word">
        ///      a word, such as "child", "apple". Must not be null.
        ///      It accepts word concatanation forms
        ///      that are common in programming languages, such as "my_child", "MyChild",
        ///      "myChild", "MY-CHILD", "CODE003-child", etc, and mostly tries to do the right thing.
        ///      ("my_children","MyChildren","myChildren", and "MY-CHILDREN", "CODE003-children" respectively)
        ///      <p>
        ///      Although this method only works for English words, it handles non-English
        ///      words gracefully (by just returning it as-is.) For example, &#x65E5;&#x672C;&#x8A9E;
        ///      will be returned as-is without modified, not "&#x65E5;&#x672C;&#x8A9E;s"
        ///      <p>
        ///      This method doesn't handle suffixes very well. For example, passing
        ///      "person56" will return "person56s", not "people56".
        /// 
        /// @return
        ///      always non-null. </param>
        public static string getPluralForm(string word)
        {
            // remember the casing of the word
            bool allUpper = true;

            // check if the word looks like an English word.
            // if we see non-ASCII characters, abort
            for (int i = 0; i < word.Length; i++)
            {
                char ch = word[i];
                if (ch >= 0x80)
                {
                    return word;
                }

                // note that this isn't the same as allUpper &= Character.isUpperCase(ch);
                allUpper &= !char.IsLower(ch);
            }

            foreach (Entry e in TABLE)
            {
                string r = e.apply(word);
                if (r != null)
                {
                    if (allUpper)
                    {
                        r = r.ToUpper();
                    }
                    return r;
                }
            }

            // failed
            return word;
        }


        /// <summary>
        /// All reserved keywords of Java. </summary>
        private static HashSet<string> reservedKeywords = new HashSet<string>();

        static JJavaName()
        {
            string[] words = new string[] { "abstract", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue", "default", "do", "double", "else", "extends", "final", "finally", "float", "for", "goto", "if", "implements", "import", "instanceof", "int", "interface", "long", "native", "new", "package", "private", "protected", "public", "return", "short", "static", "strictfp", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", "volatile", "while", "true", "false", "null", "assert", "enum" };
            foreach (string w in words)
            {
                reservedKeywords.Add(w);
            }
            string[] source = { "(.*)child", "$1children", "(.+)fe", "$1ves", "(.*)mouse", "$1mise", "(.+)f", "$1ves", "(.+)ch", "$1ches", "(.+)sh", "$1shes", "(.*)tooth", "$1teeth", "(.+)um", "$1a", "(.+)an", "$1en", "(.+)ato", "$1atoes", "(.*)basis", "$1bases", "(.*)axis", "$1axes", "(.+)is", "$1ises", "(.+)ss", "$1sses", "(.+)us", "$1uses", "(.+)s", "$1s", "(.*)foot", "$1feet", "(.+)ix", "$1ixes", "(.+)ex", "$1ices", "(.+)nx", "$1nxes", "(.+)x", "$1xes", "(.+)y", "$1ies", "(.+)", "$1s" };

            TABLE = new Entry[source.Length / 2];

            for (int i = 0; i < source.Length; i += 2)
            {
                TABLE[i / 2] = new Entry(source[i], source[i + 1]);
            }
        }


        private class Entry
        {
            private readonly Regex pattern;
            private readonly string replacement;

            public Entry(string pattern, string replacement)
            {
                this.pattern = new Regex(pattern, RegexOptions.IgnoreCase);
                this.replacement = replacement;
            }

            internal string apply(string word)
            {
                MatchCollection matcher = pattern.Matches(word);
                StringBuilder buf = new StringBuilder();               
                int last = 0;
                int p = 0;
                if (matcher.Count > 0)
                {
                    foreach (Match m in matcher)
                    {
                        string match = m.Groups[0].Value;
                        if (match.Trim().Length > 0)
                        {                            
                            buf.Append(word.Substring(last, m.Index - last));
                            buf.Append(m.Result(replacement));
                        }
                        last = m.Index + m.Length;
                    }
                    return buf.ToString();
                }                
                else
                {
                    return null;
                }
            }
        }

        private static readonly Entry[] TABLE;

    }
}
