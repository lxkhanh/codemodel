//-----------------------------------------------------------------------
// <copyright file="JFieldVar.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    /// <summary>
    /// A field that can have a <seealso cref="JDocComment"/> associated with it
    /// </summary>
    public class JFieldVar : JVar, JDocCommentable
    {

        /// <summary>
        /// javadoc comments for this JFieldVar
        /// </summary>
        private JDocComment jdoc = null;

        private readonly JDefinedClass owner;


        /// <summary>
        /// JFieldVar constructor
        /// </summary>
        /// <param name="type">
        ///        Datatype of this variable
        /// </param>
        /// <param name="name">
        ///        Name of this variable
        /// </param>
        /// <param name="init">
        ///        Value to initialize this variable to </param>
        internal JFieldVar(JDefinedClass owner, JMods mods, JType type, string name, JExpression init)
            : base(mods, type, name, init)
        {
            this.owner = owner;
        }

        public override void name(string name)
        {
            // make sure that the new name is available
            if (owner.fields().ContainsKey(name))
            {
                throw new System.ArgumentException("name " + name + " is already in use");
            }
            string oldName = base.name();
            base.name(name);
            owner.fields().Remove(oldName);
            owner.fields().Add(name, this);
        }

        /// <summary>
        /// Creates, if necessary, and returns the class javadoc for this
        /// JDefinedClass
        /// </summary>
        /// <returns> JDocComment containing javadocs for this class </returns>
        public virtual JDocComment javadoc()
        {
            if (jdoc == null)
            {
                jdoc = new JDocComment(owner.owner());
            }
            return jdoc;
        }

        public virtual void declare(JFormatter f)
        {
            if (jdoc != null)
            {
                f.g((JGenerable)jdoc);
            }
            base.declare(f);
        }


    }

}
