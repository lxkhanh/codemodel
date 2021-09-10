//-----------------------------------------------------------------------
// <copyright file="JExpression.cs">
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
    /// A Java expression.
    /// 
    /// <p>
    /// Unlike most of CodeModel, JExpressions are built bottom-up (
    /// meaning you start from leaves and then gradually build compliated expressions
    /// by combining them.)
    /// 
    /// <p>
    /// <seealso cref="JExpression"/> defines a series of composer methods,
    /// which returns a complicated expression (by often taking other <seealso cref="JExpression"/>s
    /// as parameters.
    /// For example, you can build "5+2" by
    /// <tt>JExpr.lit(5).add(JExpr.lit(2))</tt>
    /// </summary>
    public interface JExpression : JGenerable
    {
        /// <summary>
        /// Returns "-[this]" from "[this]".
        /// </summary>
        JExpression minus();

        /// <summary>
        /// Returns "![this]" from "[this]".
        /// </summary>
        JExpression not();
        /// <summary>
        /// Returns "~[this]" from "[this]".
        /// </summary>
        JExpression complement();

        /// <summary>
        /// Returns "[this]++" from "[this]".
        /// </summary>
        JExpression incr();

        /// <summary>
        /// Returns "[this]--" from "[this]".
        /// </summary>
        JExpression decr();

        /// <summary>
        /// Returns "[this]+[right]"
        /// </summary>
        JExpression plus(JExpression right);

        /// <summary>
        /// Returns "[this]-[right]"
        /// </summary>
        JExpression minus(JExpression right);

        /// <summary>
        /// Returns "[this]*[right]"
        /// </summary>
        JExpression mul(JExpression right);

        /// <summary>
        /// Returns "[this]/[right]"
        /// </summary>
        JExpression div(JExpression right);

        /// <summary>
        /// Returns "[this]%[right]"
        /// </summary>
        JExpression mod(JExpression right);

        /// <summary>
        /// Returns "[this]&lt;&lt;[right]"
        /// </summary>
        JExpression shl(JExpression right);

        /// <summary>
        /// Returns "[this]>>[right]"
        /// </summary>
        JExpression shr(JExpression right);

        /// <summary>
        /// Returns "[this]>>>[right]"
        /// </summary>
        JExpression shrz(JExpression right);

        /// <summary>
        /// Bit-wise AND '&amp;'. </summary>
        JExpression band(JExpression right);

        /// <summary>
        /// Bit-wise OR '|'. </summary>
        JExpression bor(JExpression right);

        /// <summary>
        /// Logical AND '&amp;&amp;'. </summary>
        JExpression cand(JExpression right);

        /// <summary>
        /// Logical OR '||'. </summary>
        JExpression cor(JExpression right);

        JExpression xor(JExpression right);
        JExpression lt(JExpression right);
        JExpression lte(JExpression right);
        JExpression gt(JExpression right);
        JExpression gte(JExpression right);
        JExpression eq(JExpression right);
        JExpression ne(JExpression right);

        /// <summary>
        /// Returns "[this] instanceof [right]"
        /// </summary>
        JExpression _instanceof(JType right);

        /// <summary>
        /// Returns "[this].[method]".
        /// 
        /// Arguments shall be added to the returned <seealso cref="JInvocation"/> object.
        /// </summary>
        JInvocation invoke(JMethod method);

        /// <summary>
        /// Returns "[this].[method]".
        /// 
        /// Arguments shall be added to the returned <seealso cref="JInvocation"/> object.
        /// </summary>
        JInvocation invoke(string method);
        JFieldRef @ref(JVar field);
        JFieldRef @ref(string field);
        JArrayCompRef component(JExpression index);
    }
}
