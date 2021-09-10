//-----------------------------------------------------------------------
// <copyright file="FileCodeWriter.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeModel.writer
{
    /// <summary>
    /// Writes all the source files under the specified file folder.
    /// </summary>
    public class FileCodeWriter : CodeWriter
    {

        /// <summary>
        /// The target directory to put source code. </summary>
        private readonly DirectoryInfo target;

        /// <summary>
        /// specify whether or not to mark the generated files read-only </summary>
        private readonly bool readOnly;

        /// <summary>
        /// Files that shall be marked as read only. </summary>
        private readonly ISet<FileInfo> readonlyFiles = new HashSet<FileInfo>();

        public FileCodeWriter(DirectoryInfo target)
            : this(target, false)
        {
        }

        public FileCodeWriter(DirectoryInfo target, string encoding)
            : this(target, false, encoding)
        {
        }

        public FileCodeWriter(DirectoryInfo target, bool readOnly)
            : this(target, readOnly, null)
        {
        }

        public FileCodeWriter(DirectoryInfo target, bool readOnly, string encoding)
        {
            this.target = target;
            this.readOnly = readOnly;
            this.encoding = encoding;
            if (!target.Exists)
            {
                throw new IOException(target + ": non-existent directory");
            }
        }
       
        public override Stream openBinary(JPackage pkg, string fileName)
        {
            return new System.IO.FileStream(getFile(pkg, fileName).FullName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        }
       
        protected internal virtual FileInfo getFile(JPackage pkg, string fileName)
        {
            DirectoryInfo  dir;
            if (pkg.isUnnamed)
            {
                dir = target;
            }
            else
            {
                dir = new DirectoryInfo(Path.Combine(target.FullName, toDirName(pkg)));
            }

            if (!dir.Exists)
            {
                Directory.CreateDirectory(Path.Combine(target.FullName, toDirName(pkg)));
            }

            FileInfo fn = new FileInfo(Path.Combine(dir.FullName, fileName));

            if (fn.Exists)
            {
                try {
                    fn.Delete();
                }
                catch (IOException ex) 
                {
                    throw new IOException(fn + ": Can't delete previous version");
                }
            }


            if (readOnly)
            {
                readonlyFiles.Add(fn);
            }
            return fn;
        }
     
        public override void close()
        {
            // mark files as read-onnly if necessary
            foreach (FileInfo f in readonlyFiles)
            {
                f.IsReadOnly = true;
            }
        }

        /// <summary>
        /// Converts a package name to the directory name. </summary>
        private static string toDirName(JPackage pkg)
        {
            return pkg.name.Replace('.', System.IO.Path.DirectorySeparatorChar);
        }

    }
}
