//-----------------------------------------------------------------------
// <copyright file="JDocComment.cs">
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
    /// JavaDoc comment.
    /// 
    /// <p>
    /// A javadoc comment consists of multiple parts. There's the main part (that comes the first in
    /// in the comment section), then the parameter parts (@param), the return part (@return),
    /// and the throws parts (@throws).
    /// 
    /// TODO: it would be nice if we have JComment class and we can derive this class from there.
    /// </summary>
    public class JDocComment : JCommentPart, JGenerable
    {
        private const long serialVersionUID = 1L;

        /** list of @param tags */
        private readonly IDictionary<string, JCommentPart> atParams = new Dictionary<string, JCommentPart>();

        /// <summary>
		/// list of xdoclets </summary>
		private readonly IDictionary<string, IDictionary<string, string>> atXdoclets = new Dictionary<string, IDictionary<string, string>>();

		/// list of <exception cref="tags"> </exception>
		private readonly IDictionary<JClass, JCommentPart> atThrows = new Dictionary<JClass, JCommentPart>();

		/// The <returns> tag part. </returns>
		private JCommentPart atReturn = null;

		/// The @deprecated tag 
		private JCommentPart atDeprecated = null;

		private readonly JCodeModel owner;


		public JDocComment(JCodeModel owner)
		{
			this.owner = owner;
		}

		public override JCommentPart append(object o)
		{
			add(o);
			return this;
		}

		/// Append a text to a <param name="tag"> to the javadoc </param>
		public virtual JCommentPart addParam(string param)
		{
            JCommentPart p = null;
            if (atParams != null && atParams.ContainsKey(param)) {
                p = atParams[param];
            }
			
			if (p == null)
			{
				atParams.Add(param,p = new JCommentPart());
			}
			return p;
		}

		/// Append a text to an <param name="tag."> </param>
		public virtual JCommentPart addParam(JVar param)
		{
			return addParam(param.name());
		}


		/// add an <exception cref="tag"> to the javadoc </exception>
		public virtual JCommentPart addThrows(Type exception)
		{
			return addThrows(owner.@ref(exception));
		}

		/// add an <exception cref="tag"> to the javadoc </exception>
		public virtual JCommentPart addThrows(JClass exception)
		{
            JCommentPart p = null;
            if (atThrows != null && atThrows.ContainsKey(exception)) {
                p = atThrows[exception];
            }
		
			if (p == null)
			{
				atThrows.Add(exception,p = new JCommentPart());
			}
			return p;
		}

		/// Appends a text to <returns> tag. </returns>
		public virtual JCommentPart addReturn()
		{
			if (atReturn == null)
			{
				atReturn = new JCommentPart();
			}
			return atReturn;
		}

		/// add an @deprecated tag to the javadoc, with the associated message. 
		public virtual JCommentPart addDeprecated()
		{
			if (atDeprecated == null)
			{
				atDeprecated = new JCommentPart();
			}
			return atDeprecated;
		}

		/// <summary>
		/// add an xdoclet.
		/// </summary>
		public virtual IDictionary<string, string> addXdoclet(string name)
		{
            IDictionary<string, string> p = null;
            if (atXdoclets != null && atXdoclets.ContainsKey(name)) {
                p = atXdoclets[name];
            }			
			if (p == null)
			{
				atXdoclets.Add(name,p = new Dictionary<string, string>());
			}
			return p;
		}

		/// <summary>
		/// add an xdoclet.
		/// </summary>
		public virtual IDictionary<string, string> addXdoclet(string name, IDictionary<string, string> attributes)
		{
            IDictionary<string, string> p = null;
            if (atXdoclets != null && atXdoclets.ContainsKey(name))
            {
                p = atXdoclets[name];
            }			
			if (p == null)
			{
				atXdoclets.Add(name,p = new Dictionary<string, string>());
			}
			//p.putAll(attributes);
            foreach (var attribute in attributes) {
                if (!p.ContainsKey(attribute.Key))
                {
                    p.Add(attribute.Key, attribute.Value);
                }
            }
			return p;
		}

		/// <summary>
		/// add an xdoclet.
		/// </summary>
		public virtual IDictionary<string, string> addXdoclet(string name, string attribute, string value)
		{
            IDictionary<string, string> p = null;
            if (atXdoclets != null && atXdoclets.ContainsKey(name))
            {
                p = atXdoclets[name];
            }			
			if (p == null)
			{
				atXdoclets.Add(name,p = new Dictionary<string, string>());
			}
			p.Add(attribute, value);
			return p;
		}

		public virtual void generate(JFormatter f)
		{
			// I realized that we can't use StringTokenizer because
			// this will recognize multiple \n as one token.

			f.p("/**").nl();

			format(f," * ");

			f.p(" * ").nl();
			foreach (KeyValuePair<string, JCommentPart> e in atParams)
			{
				f.p(" * @param ").p(e.Key).nl();
				e.Value.format(f,INDENT);
			}
			if (atReturn != null)
			{
				f.p(" * @return").nl();
				atReturn.format(f,INDENT);
			}
			foreach (KeyValuePair<JClass, JCommentPart> e in atThrows)
			{
				f.p(" * @throws ").t(e.Key).nl();
				e.Value.format(f,INDENT);
			}
			if (atDeprecated != null)
			{
				f.p(" * @deprecated").nl();
				atDeprecated.format(f,INDENT);
			}
			foreach (KeyValuePair<string, IDictionary<string, string>> e in atXdoclets)
			{
				f.p(" * @").p(e.Key);
				if (e.Value != null)
				{
					foreach (KeyValuePair<string, string> a in e.Value)
					{
						f.p(" ").p(a.Key).p("= \"").p(a.Value).p("\"");
					}
				}
				f.nl();
			}
			f.p(" */").nl();
		}

		private const string INDENT = " *     ";
	}
}
