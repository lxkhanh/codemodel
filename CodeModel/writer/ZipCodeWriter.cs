//-----------------------------------------------------------------------
// <copyright file="ZipCodeWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;

namespace CodeModel.writer
{
    public class ZipCodeWriter : CodeWriter
    {
        /// <param name="target">
        ///      Zip file will be written to this stream. </param>
        public ZipCodeWriter(System.IO.Stream target)
        {
            zip = new ZipOutputStream(target);
            // nullify the close method.
            filter = new FilterOutputStreamAnonymousInnerClass(this, zip);
        }

        private class FilterOutputStreamAnonymousInnerClass : System.IO.MemoryStream
        {
            private readonly ZipCodeWriter outerInstance;

            public FilterOutputStreamAnonymousInnerClass(ZipCodeWriter outerInstance, ZipOutputStream zip) : base()
            {
                this.outerInstance = outerInstance;
            }
            public void close()
            {
            }
        }

        private readonly ZipOutputStream zip;

        private readonly System.IO.Stream filter;

        public override Stream openBinary(JPackage pkg, string fileName)
        {
            string name = fileName;
            if (!pkg.isUnnamed)
            {
                name = toDirName(pkg) + name;
            }

            zip.PutNextEntry(name);
            return filter;
        }

        /// <summary>
        /// Converts a package name to the directory name. </summary>
        private static string toDirName(JPackage pkg)
        {
            return pkg.name.Replace('.', '/') + '/';
        }
       
        public override void close()
        {
            zip.Close();
        }

    }
}
