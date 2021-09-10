//-----------------------------------------------------------------------
// <copyright file="JAssignmentTarget.cs">
//     Copyright (c) emotive GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeModel
{
    /**
     * Marker interface for code components that can be placed to
     * the left of '=' in an assignment.
     * 
     * A left hand value can always be a right hand value, so
     * this interface derives from {@link JExpression}. 
     */
    public interface JAssignmentTarget: JGenerable, JExpression {
        JExpression assign(JExpression rhs);
        JExpression assignPlus(JExpression rhs);
    }
}
