//-----------------------------------------------------------------------
// <copyright file="ClassType.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
   
    public sealed class ClassType {
    /**
     * The keyword used to declare this type.
     */
    public string declarationToken;
    
    private ClassType(string token) {
        this.declarationToken = token;
    }
    
    public static ClassType CLASS = new ClassType("class");
    
    public static ClassType INTERFACE = new ClassType("interface");
    
    public static ClassType ANNOTATION_TYPE_DECL = new ClassType("@interface");
    
    public static ClassType ENUM = new ClassType("enum");

    }
}
