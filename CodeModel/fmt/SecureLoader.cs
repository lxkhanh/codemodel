//-----------------------------------------------------------------------
// <copyright file="SecureLoader.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace CodeModel.fmt
{
    /// <summary>
    /// Class defined for safe calls of getClassLoader methods of any kind (context/system/class
    /// classloader. This MUST be package private and defined in every package which 
    /// uses such invocations.
    /// @author snajper
    /// </summary>
    internal class SecureLoader
    {
        private static Assembly assembly;
        internal static Assembly getContextClassLoader()
        {
                       
            return Assembly.GetEntryAssembly();
        }
        internal static void setContextClassLoader(Assembly cl)
        {                     
            assembly = cl;
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
