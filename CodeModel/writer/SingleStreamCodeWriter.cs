//-----------------------------------------------------------------------
// <copyright file="SingleStreamCodeWriter.cs">
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
    public class SingleStreamCodeWriter : CodeWriter
    {

        private readonly Stream @out;

        /// <param name="os">
        ///      This stream will be closed at the end of the code generation. </param>
        public SingleStreamCodeWriter(System.IO.FileStream os)
        {
            @out = os;
        }

        public override Stream openBinary(JPackage pkg, string fileName)
        {
            string pkgName = pkg.name;
            if (pkgName.Length != 0)
            {
                pkgName += '.';
            }

            //@out.WriteLine("-----------------------------------" + pkgName + fileName + "-----------------------------------");

            return @out;
        }
  
        public override void close()
        {
            @out.Close();
        }

    }
}
