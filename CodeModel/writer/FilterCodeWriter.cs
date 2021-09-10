//-----------------------------------------------------------------------
// <copyright file="FilterCodeWriter.cs">
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
    public class FilterCodeWriter : CodeWriter
    {
        protected internal CodeWriter core;

        public FilterCodeWriter(CodeWriter core)
        {
            this.core = core;
        }
       
        public override Stream openBinary(JPackage pkg, string fileName)
        {
            return core.openBinary(pkg, fileName);
        }

        public override TextWriter openSource(JPackage pkg, string fileName)
        {
            return core.openSource(pkg, fileName);
        }
     
        public override void close()
        {
            core.close();
        }
    }
}
