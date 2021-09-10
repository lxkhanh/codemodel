//-----------------------------------------------------------------------
// <copyright file="JPropertyFile.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CodeModel.fmt
{
   public class JPropertyFile : JResourceFile
	{
		public JPropertyFile(string name) : base(name)
		{
		}

        private Hashtable data = new Hashtable();
        

		/// <summary>
		/// Adds key/value pair into the property file.
		/// If you call this method twice with the same key,
		/// the old one is overriden by the new one.
		/// </summary>
		public virtual void add(string key, string value)
		{
			data.Add(key,value);
		}

		// TODO: method to iterate values in data?
		// TODO: should we rather expose Properties object directly via
		// public Properties body() { return data; } ?

        public override void build(Stream @out)
		{
            var binFormatter = new BinaryFormatter();            
            binFormatter.Serialize(@out, data);         
		}
	}

}
