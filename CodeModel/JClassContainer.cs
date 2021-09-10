//-----------------------------------------------------------------------
// <copyright file="JClassContainer.cs">
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
    /// The common aspect of a package and a class.
    /// </summary>
    public interface JClassContainer
    {

        /// <summary>
        /// Returns true if the container is a class.
        /// </summary>
        bool isClass { get; }
        /// <summary>
        /// Returns true if the container is a package.
        /// </summary>
        bool isPackage { get; }

        /// <summary>
        /// Add a new class to this package/class.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this class declaration
        /// </param>
        /// <param name="name">
        ///        Name of class to be added to this package
        /// </param>
        /// <returns> Newly generated class
        /// </returns>
        JDefinedClass _class(int mods, string name);

        /// <summary>
        /// Add a new public class to this class/package.
        /// </summary>
        JDefinedClass _class(string name);

        /// <summary>
        /// Add an interface to this class/package.
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for this interface declaration
        /// </param>
        /// <param name="name">
        ///        Name of interface to be added to this package
        /// </param>
        /// <returns> Newly generated interface
        /// </returns>      
        JDefinedClass _interface(int mods, string name);

        /// <summary>
        /// Adds a public interface to this package.
        /// </summary>       
        JDefinedClass _interface(string name);

        /// <summary>
        /// Create a new class or a new interface.
        /// 
        /// @deprecated
        ///      use <seealso cref="#_class(int, String, ClassType)"/> 
        /// </summary>       
        JDefinedClass _class(int mods, string name, bool isInterface);

        /// <summary>
        /// Creates a new class/enum/interface/annotation.
        /// </summary>       
        JDefinedClass _class(int mods, string name, ClassType kind);


        /// <summary>
        /// Returns an iterator that walks the nested classes defined in this
        /// class.
        /// </summary>
        IEnumerator<JDefinedClass> classes();

        /// <summary>
        /// Parent JClassContainer.
        /// 
        /// If this is a package, this method returns a parent package,
        /// or null if this package is the root package.
        /// 
        /// If this is an outer-most class, this method returns a package
        /// to which it belongs.
        /// 
        /// If this is an inner class, this method returns the outer
        /// class.
        /// </summary>
        JClassContainer parentContainer();

        /// <summary>
        /// Gets the nearest package parent.
        /// 
        /// <p>
        /// If <tt>this.isPackage()</tt>, then return <tt>this</tt>.
        /// </summary>
        JPackage package { get; }

        /// <summary>
        /// Get the root code model object.
        /// </summary>
        JCodeModel owner();

        /// <summary>
        /// Add an annotationType Declaration to this package </summary>
        /// <param name="name">
        ///      Name of the annotation Type declaration to be added to this package
        /// @return
        ///      newly created Annotation Type Declaration </param>        
        JDefinedClass _annotationTypeDeclaration(string name);

        /// <summary>
        /// Add a public enum to this package </summary>
        /// <param name="name">
        ///      Name of the enum to be added to this package
        /// @return
        ///      newly created Enum </param>
        /// <exception cref="JClassAlreadyExistsException">
        ///      When the specified class/interface was already created.
        ///  </exception>     
        JDefinedClass _enum(string name);
    }
}
