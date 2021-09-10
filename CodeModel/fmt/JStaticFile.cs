//-----------------------------------------------------------------------
// <copyright file="JStaticFile.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace CodeModel.fmt
{
    public sealed class JStaticFile : JResourceFile
    {

        private readonly Assembly classLoader;
        private readonly string resourceName;
        private readonly bool isResource;

        public JStaticFile(string _resourceName)
            : this(_resourceName, !_resourceName.EndsWith(".java", StringComparison.Ordinal))
        {
        }

        public JStaticFile(string _resourceName, bool isResource)
            : this(SecureLoader.getClassClassLoader(typeof(JStaticFile)), _resourceName, isResource)
        {
        }

        /// <param name="isResource">
        ///      false if this is a Java source file. True if this is other resource files. </param>
        public JStaticFile(Assembly _classLoader, string _resourceName, bool isResource)
            : base(_resourceName.Substring(_resourceName.LastIndexOf('/') + 1))
        {
            this.classLoader = _classLoader;
            this.resourceName = _resourceName;
            this.isResource = isResource;
        }

        protected internal bool Resource
        {
            get
            {
                return isResource;
            }
        }
       
        public override void build(System.IO.Stream os)
        {
            BinaryReader dis = new BinaryReader(classLoader.GetManifestResourceStream(resourceName));

            byte[] buf = new byte[256];
            int sz;
            while ((sz = dis.Read(buf, 0, buf.Length)) > 0)
            {
                os.Write(buf, 0, sz);
            }

            dis.Close();
        }

    }

}
