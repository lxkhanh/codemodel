//-----------------------------------------------------------------------
// <copyright file="JMods.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel
{
    /// <summary>
    /// Modifier groups.
    /// </summary>
    public class JMods : JGenerable
    {

        //
        // mask
        //
        private static int VAR = JMod.FINAL;
        private static int FIELD = (JMod.PUBLIC | JMod.PRIVATE | JMod.PROTECTED | JMod.STATIC | JMod.FINAL | JMod.TRANSIENT | JMod.VOLATILE);
        private static int METHOD = (JMod.PUBLIC | JMod.PRIVATE | JMod.PROTECTED | JMod.FINAL | JMod.ABSTRACT | JMod.STATIC | JMod.NATIVE | JMod.SYNCHRONIZED);
        private static int CLASS = (JMod.PUBLIC | JMod.PRIVATE | JMod.PROTECTED | JMod.STATIC | JMod.FINAL | JMod.ABSTRACT);
        private static int INTERFACE = JMod.PUBLIC;
        /// <summary>
        /// bit-packed representation of modifiers. </summary>
        private int mods;

        private JMods(int mods)
        {
            this.mods = mods;
        }

        /// <summary>
        /// Gets the bit-packed representaion of modifiers.
        /// </summary>
        public virtual int value
        {
            get
            {
                return mods;
            }
        }

        private static void check(int mods, int legal, string what)
        {
            if ((mods & ~legal) != 0)
            {
                throw new System.ArgumentException("Illegal modifiers for " + what + ": " + new JMods(mods).ToString());
            }
            /* ## check for illegal combinations too */
        }

        public static JMods forVar(int mods)
        {
            check(mods, VAR, "variable");
            return new JMods(mods);
        }

        public static JMods forField(int mods)
        {
            check(mods, FIELD, "field");
            return new JMods(mods);
        }

        public static JMods forMethod(int mods)
        {
            check(mods, METHOD, "method");
            return new JMods(mods);
        }

        public static JMods forClass(int mods)
        {
            check(mods, CLASS, "class");
            return new JMods(mods);
        }

        public static JMods forInterface(int mods)
        {
            check(mods, INTERFACE, "class");
            return new JMods(mods);
        }

        public virtual bool isAbstract
        {
            get
            {
                return (mods & JMod.ABSTRACT) != 0;
            }
        }

        public virtual bool isNative
        {
            get
            {
                return (mods & JMod.NATIVE) != 0;
            }
        }

        public virtual bool isSynchronized
        {
            get
            {
                return (mods & JMod.SYNCHRONIZED) != 0;
            }          
        }
        
        public void setSynchronized(bool newValue)
        {
            setFlag(JMod.SYNCHRONIZED, newValue);
        }

        public virtual void setPrivate()
        {
            setFlag(JMod.PUBLIC, false);
            setFlag(JMod.PROTECTED, false);
            setFlag(JMod.PRIVATE, true);
        }

        public virtual void setProtected()
        {
            setFlag(JMod.PUBLIC, false);
            setFlag(JMod.PROTECTED, true);
            setFlag(JMod.PRIVATE, false);
        }

        public virtual void setPublic()
        {
            setFlag(JMod.PUBLIC, true);
            setFlag(JMod.PROTECTED, false);
            setFlag(JMod.PRIVATE, false);
        }

        public virtual bool setFinal
        {
            set
            {
                setFlag(JMod.FINAL, value);
            }
        }

        private void setFlag(int bit, bool newValue)
        {
            mods = (mods & ~bit) | (newValue ? bit : 0);
        }

        public virtual void generate(JFormatter f)
        {
            if ((mods & JMod.PUBLIC) != 0)
            {
                f.p("public");
            }
            if ((mods & JMod.PROTECTED) != 0)
            {
                f.p("protected");
            }
            if ((mods & JMod.PRIVATE) != 0)
            {
                f.p("private");
            }
            if ((mods & JMod.FINAL) != 0)
            {
                f.p("final");
            }
            if ((mods & JMod.STATIC) != 0)
            {
                f.p("static");
            }
            if ((mods & JMod.ABSTRACT) != 0)
            {
                f.p("abstract");
            }
            if ((mods & JMod.NATIVE) != 0)
            {
                f.p("native");
            }
            if ((mods & JMod.SYNCHRONIZED) != 0)
            {
                f.p("synchronized");
            }
            if ((mods & JMod.TRANSIENT) != 0)
            {
                f.p("transient");
            }
            if ((mods & JMod.VOLATILE) != 0)
            {
                f.p("volatile");
            }
        }

        public override string ToString()
        {
            StreamWriter s = new StreamWriter(new MemoryStream());
            JFormatter f = new JFormatter(s);
            this.generate(f);
            return s.ToString();
        }
    }
}
