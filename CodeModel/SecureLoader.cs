//-----------------------------------------------------------------------
// <copyright file="SecureLoader.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace CodeModel
{
    /// <summary>
    /// Class defined for safe calls of getClassLoader methods of any kind (context/system/class
    /// classloader. This MUST be package private and defined in every package which 
    /// uses such invocations.

    internal class SecureLoader
    {
        private static Assembly assembly;
        internal static Assembly contextClassLoader
        {
            get
            {             
                return Assembly.GetExecutingAssembly();
            }
            set
            {              
                assembly = value;
            }
        }
       
        internal static Assembly getClassClassLoader(Type c)
        {
          
            return c.Assembly;
        }

        internal static Assembly getSystemClassLoader()
        {           
            return Assembly.GetEntryAssembly();
        }
      
    }

}
