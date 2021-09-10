//-----------------------------------------------------------------------
// <copyright file="JFormatter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace CodeModel
{
    /// <summary>
    /// This is a utility class for managing indentation and other basic
    /// formatting for PrintWriter.
    /// </summary>
    public sealed class JFormatter
    {
        /// <summary>
        /// all classes and ids encountered during the collection mode * </summary>
        /// <summary>
        /// map from short type name to ReferenceList (list of JClass and ids sharing that name) * </summary>
        private Dictionary<string, ReferenceList> collectedReferences;

        /// <summary>
        /// set of imported types (including package java types, eventhough we won't generate imports for them) </summary>
        private HashSet<JClass> importedClasses;

        private enum Mode
        {
            /// <summary>
            /// Collect all the type names and identifiers.
            /// In this mode we don't actually generate anything.
            /// </summary>
            COLLECTING,
            /// <summary>
            /// Print the actual source code.
            /// </summary>
            PRINTING
        }

        /// <summary>
        /// The current running mode.
        /// Set to PRINTING so that a casual client can use a formatter just like before.
        /// </summary>
        private Mode mode = Mode.PRINTING;

        /// <summary>
        /// Current number of indentation strings to print
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// String to be used for each indentation.
        /// Defaults to four spaces.
        /// </summary>
        private readonly string indentSpace;

        /// <summary>
        /// Stream associated with this JFormatter
        /// </summary>
        private readonly TextWriter pw;

        /// <summary>
        /// Creates a JFormatter.
        /// </summary>
        /// <param name="s">
        ///        PrintWriter to JFormatter to use.
        /// </param>
        /// <param name="space">
        ///        Incremental indentation string, similar to tab value. </param>
        public JFormatter(TextWriter s, string space)
        {
            pw = s;
            indentSpace = space;
            collectedReferences = new Dictionary<string, ReferenceList>();
            //ids = new HashSet<String>();
            importedClasses = new HashSet<JClass>();
        }

        /// <summary>
        /// Creates a formatter with default incremental indentations of
        /// four spaces.
        /// </summary>
        public JFormatter(TextWriter s)
            : this(s, "    ")
        {
        }

        /// <summary>
        /// Creates a formatter with default incremental indentations of
        /// four spaces.
        /// </summary>
        public JFormatter(Stream w): this(new StreamWriter(w))
        {
        }

        /// <summary>
        /// Closes this formatter.
        /// </summary>
        public void close()
        {
            pw.Close();
        }

        /// <summary>
        /// Returns true if we are in the printing mode,
        /// where we actually produce text.
        /// 
        /// The other mode is the "collecting mode'
        /// </summary>
        public bool isPrinting()
        {          
           return mode == Mode.PRINTING;
        }

        /// <summary>
        /// Decrement the indentation level.
        /// </summary>
        public JFormatter o()
        {
            indentLevel--;
            return this;
        }

        /// <summary>
        /// Increment the indentation level.
        /// </summary>
        public JFormatter i()
        {
            indentLevel++;
            return this;
        }

        private bool needSpace(char c1, char c2)
        {
            if ((c1 == ']') && (c2 == '{'))
            {
                return true;
            }
            if (c1 == ';')
            {
                return true;
            }
            if (c1 == CLOSE_TYPE_ARGS)
            {
                // e.g., "public Foo<Bar> test;"
                if (c2 == '(') // but not "new Foo<Bar>()"
                {
                    return false;
                }
                return true;
            }
            if ((c1 == ')') && (c2 == '{'))
            {
                return true;
            }
            if ((c1 == ',') || (c1 == '='))
            {
                return true;
            }
            if (c2 == '=')
            {
                return true;
            }
            if (char.IsDigit(c1))
            {
                if ((c2 == '(') || (c2 == ')') || (c2 == ';') || (c2 == ','))
                {
                    return false;
                }
                return true;
            }

            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            if (provider.IsValidIdentifier(Convert.ToString(c1)))
            {
                switch (c2)
                {
                    case '{':
                    case '}':
                    case '+':
                    case '>':
                    case '@':
                        return true;
                    default:
                        return provider.IsValidIdentifier(Convert.ToString(c2));
                }
            }
            if (provider.IsValidIdentifier(Convert.ToString(c2)))
            {
                switch (c1)
                {
                    case ']':
                    case ')':
                    case '}':
                    case '+':
                        return true;
                    default:
                        return false;
                }
            }
            if (char.IsDigit(c2))
            {
                if (c1 == '(')
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private char lastChar = '\0';
        private bool atBeginningOfLine = true;

        private void spaceIfNeeded(char c)
        {
            if (atBeginningOfLine)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    pw.Write(indentSpace);
                }
                atBeginningOfLine = false;
            }
            else if ((lastChar != 0) && needSpace(lastChar, c))
            {
                pw.Write(' ');
            }
        }

        /// <summary>
        /// Write a char into the stream
        /// </summary>
        /// <param name="c"> the char </param>
        public JFormatter p(char c)
        {
            if (mode == Mode.PRINTING)
            {
                if (c == CLOSE_TYPE_ARGS)
                {
                    pw.Write('>');
                }
                else
                {
                    spaceIfNeeded(c);
                    pw.Write(c);
                }
                lastChar = c;
            }
            return this;
        }

        /// <summary>
        /// Write a String into the stream
        /// </summary>
        /// <param name="s"> the String </param>
        public JFormatter p(string s)
        {
            if (mode == Mode.PRINTING)
            {
                spaceIfNeeded(s[0]);
                pw.Write(s);
                lastChar = s[s.Length - 1];
            }
            return this;
        }

        public JFormatter t(JType type)
        {
            if (type.isReference())
            {
                return t((JClass)type);
            }
            else
            {
                return g(type);
            }
        }

        /// <summary>
        /// Print a type name.
        /// 
        /// <p>
        /// In the collecting mode we use this information to
        /// decide what types to import and what not to.
        /// </summary>
        public JFormatter t(JClass type)
        {
            switch (mode)
            {
                case Mode.PRINTING:
                    // many of the JTypes in this list are either primitive or belong to package java
                    // so we don't need a FQCN
                    if (importedClasses.Contains(type))
                    {
                        p(type.name()); // FQCN imported or not necessary, so generate short name
                    }
                    else
                    {
                        if (type.outer() != null)
                        {
                            t(type.outer()).p('.').p(type.name());
                        }
                        else
                        {
                            p(type.fullName()); // collision was detected, so generate FQCN
                        }
                    }
                    break;
                case Mode.COLLECTING:                   
                    string shortName = type.name();
                    if (collectedReferences.ContainsKey(shortName))
                    {
                        collectedReferences[shortName].add(type);
                    }
                    else
                    {
                        ReferenceList tl = new ReferenceList(this);
                        tl.add(type);
                        collectedReferences.Add(shortName, tl);
                    }
                    break;
            }
            return this;
        }

        /// <summary>
        /// Print an identifier
        /// </summary>
        public JFormatter id(string id)
        {
            switch (mode)
            {
                case Mode.PRINTING:
                    p(id);
                    break;
                case Mode.COLLECTING:
                    // see if there is a type name that collides with this id
                    if (collectedReferences.ContainsKey(id))
                    {
                        if (!(collectedReferences[id].classes.Count() == 0))
                        {
                            foreach (JClass type in collectedReferences[id].classes)
                            {
                                if (type.outer() != null)
                                {
                                    collectedReferences[id].id = false;
                                    return this;
                                }
                            }
                        }
                        collectedReferences[id].id = true;
                    }
                    else
                    {
                        // not a type, but we need to create a place holder to
                        // see if there might be a collision with a type
                        ReferenceList tl = new ReferenceList(this);
                        tl.id = true;
                        collectedReferences.Add(id, tl);
                    }
                    break;
            }
            return this;
        }

        /// <summary>
        /// Print a new line into the stream
        /// </summary>
        public JFormatter nl()
        {
            if (mode == Mode.PRINTING)
            {
                pw.WriteLine();
                lastChar = '\0';
                atBeginningOfLine = true;
            }
            return this;
        }

        /// <summary>
        /// Cause the JGenerable object to generate source for iteself
        /// </summary>
        /// <param name="g"> the JGenerable object </param>
        public JFormatter g(JGenerable g)
        {
            g.generate(this);
            return this;
        }

        /// <summary>
        /// Produces <seealso cref="JGenerable"/>s separated by ','
        /// </summary>
        public JFormatter g<T1>(ICollection<T1> list) where T1 : JGenerable
        {
            bool first = true;
            if (list.Count > 0)
            {
                foreach (JGenerable item in list)
                {
                    if (!first)
                    {
                        p(',');
                    }
                    g(item);
                    first = false;
                }
            }
            return this;
        }

        /// <summary>
        /// Cause the JDeclaration to generate source for itself
        /// </summary>
        /// <param name="d"> the JDeclaration object </param>
        public JFormatter d(JDeclaration d)
        {
            d.declare(this);
            return this;
        }

        /// <summary>
        /// Cause the JStatement to generate source for itself
        /// </summary>
        /// <param name="s"> the JStatement object </param>
        public JFormatter s(JStatement s)
        {
            s.state(this);
            return this;
        }

        /// <summary>
        /// Cause the JVar to generate source for itself
        /// </summary>
        /// <param name="v"> the JVar object </param>
        public JFormatter b(JVar v)
        {
            v.bind(this);
            return this;
        }

        /// <summary>
        /// Generates the whole source code out of the specified class.
        /// </summary>
        internal void write(JDefinedClass c)
        {
            // first collect all the types and identifiers
            mode = Mode.COLLECTING;
            d(c);

            javaLang = c.owner()._package("java.lang");

            // collate type names and identifiers to determine which types can be imported
            foreach (ReferenceList tl in collectedReferences.Values)
            {
                if (!tl.collisions(c) && !tl.id)
                {
                    Debug.Assert(tl.classes.Count == 1);

                    // add to list of collected types
                    importedClasses.Add(tl.classes[0]);
                }
            }

            // the class itself that we will be generating is always accessible
            importedClasses.Add(c);

            // then print the declaration
            mode = Mode.PRINTING;

            Debug.Assert(c.parentContainer().isPackage, "this method is only for a pacakge-level class");          
            JPackage pkg = (JPackage)c.parentContainer();
            if (!pkg.isUnnamed)
            {
                nl().d(pkg);
                nl();
            }

            // generate import statements
            JClass[] imports = importedClasses.ToArray();
            //Arrays.sort(imports);
            foreach (JClass clazz in imports)
            {
                // suppress import statements for primitive types, built-in types,
                // types in the root package, and types in
                // the same package as the current type
                if (!supressImport(clazz, c))
                {
                    JClass clazz1 = clazz;
                    if (clazz is JNarrowedClass)
                    {
                        clazz1 = (JClass)clazz.erasure();
                    }

                    p("import").p(clazz1.fullName()).p(';').nl();
                }
            }
            nl();

            d(c);
        }

        /// <summary>
        /// determine if an import statement should be supressed
        /// </summary>
        /// <param name="clazz"> JType that may or may not have an import </param>
        /// <param name="c"> JType that is the current class being processed </param>
        /// <returns> true if an import statement should be suppressed, false otherwise </returns>
        private bool supressImport(JClass clazz, JClass c)
        {
            if (clazz is JNarrowedClass)
            {
                clazz = (JClass)clazz.erasure();
            }
            if (clazz is JAnonymousClass)
            {
                clazz = clazz._extends();
            }

            if (clazz._package().isUnnamed)
            {
                return true;
            }
           
            string packageName = clazz._package().name;
            if (packageName.Equals("java.lang"))
            {
                return true; // no need to explicitly import java.lang classes
            }

            if (clazz._package() == c._package())
            {
                // inner classes require an import stmt.
                // All other pkg local classes do not need an
                // import stmt for ref.
                if (clazz.outer() == null)
                {
                    return true; // no need to explicitly import a class into itself
                }
            }
            return false;
        }

        private JPackage javaLang;



        /// <summary>
        /// Special character token we use to differenciate '>' as an operator and
        /// '>' as the end of the type arguments. The former uses '>' and it requires
        /// a preceding whitespace. The latter uses this, and it does not have a preceding
        /// whitespace.
        /// </summary>
        /*package*/
        internal static readonly char CLOSE_TYPE_ARGS = '\uFFFF';

        /// <summary>
        /// Used during the optimization of class imports.
        /// 
        /// List of <seealso cref="JClass"/>es whose short name is the same.
        /// 
        /// @author Ryan.Shoemaker@Sun.COM
        /// </summary>
        internal sealed class ReferenceList
        {
            private readonly JFormatter outerInstance;

            public ReferenceList(JFormatter outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            internal readonly List<JClass> classes = new List<JClass>();

            /// <summary>
            /// true if this name is used as an identifier (like a variable name.) * </summary>
            internal bool id;

            /// <summary>
            /// Returns true if the symbol represented by the short name
            /// is "importable".
            /// </summary>
            public bool collisions(JDefinedClass enclosingClass)
            {
                // special case where a generated type collides with a type in package java

                // more than one type with the same name
                if (classes.Count > 1)
                {
                    return true;
                }

                // an id and (at least one) type with the same name
                if (id && classes.Count != 0)
                {
                    return true;
                }

                foreach (JClass c in classes)
                {
                    JClass c1 = c;
                    if (c1 is JAnonymousClass)
                    {
                        c1 = c._extends();
                    }
                    if (c1._package() == outerInstance.javaLang)
                    {
                        // make sure that there's no other class with this name within the same package
                        IEnumerator<JDefinedClass> itr = enclosingClass._package().classes();
                        while (itr.MoveNext())
                        {
                            // even if this is the only "String" class we use,
                            // if the class called "String" is in the same package,
                            // we still need to import it.
                            JDefinedClass n = itr.Current;
                            if (n.name().Equals(c.name()))
                            {
                                return true; //collision
                            }
                        }
                    }
                    if (c.outer() != null)
                    {
                        return true; // avoid importing inner class to work around 6431987. Also see jaxb issue 166
                    }
                }

                return false;
            }

            public void add(JClass clazz)
            {
                if (!classes.Contains(clazz))
                {
                    classes.Add(clazz);
                }
            }

            public IList<JClass> Classes
            {
                get
                {
                    return classes;
                }
            }

            public bool Id
            {
                set
                {
                    id = value;
                }
                get
                {
                    return id && classes.Count == 0;
                }
            }

        }
    }
}
