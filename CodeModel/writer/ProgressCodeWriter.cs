//-----------------------------------------------------------------------
// <copyright file="ProgressCodeWriter.cs">
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
    public class ProgressCodeWriter : FilterCodeWriter
    {
        public ProgressCodeWriter(CodeWriter output, StreamWriter progress)
            : base(output)
        {
            this.progress = progress;
            if (progress == null)
            {
                throw new System.ArgumentException();
            }
        }

        private readonly StreamWriter progress;

        
        public override Stream openBinary(JPackage pkg, string fileName)
        {
            report(pkg, fileName);
            return base.openBinary(pkg, fileName);
        }
       
        public override TextWriter openSource(JPackage pkg, string fileName)
        {
            report(pkg, fileName);
            return base.openSource(pkg, fileName);
        }

        private void report(JPackage pkg, string fileName)
        {
            if (pkg.isUnnamed)
            {
                progress.WriteLine(fileName);
            }
            else
            {
                progress.WriteLine(pkg.name.Replace('.', System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar + fileName);
            }
        }

    }
}
