//-----------------------------------------------------------------------
// <copyright file="JBlock.cs">
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
    /// A block of Java code, which may contain statements and local declarations.
    /// 
    /// <para>
    /// <seealso cref="JBlock"/> contains a large number of factory methods that creates new
    /// statements/declarations. Those newly created statements/declarations are
    /// inserted into the <seealso cref="#pos() "current position""/>. The position advances
    /// one every time you add a new instruction.
    /// </para>
    /// </summary>
    public class JBlock : JGenerable, JStatement
    {

        /// <summary>
        /// Declarations and statements contained in this block.
        /// Either <seealso cref="JStatement"/> or <seealso cref="JDeclaration"/>.
        /// </summary>
        private readonly IList<object> content = new List<object>();

        /// <summary>
        /// Whether or not this block must be braced and indented
        /// </summary>
        private bool bracesRequired = true;
        private bool indentRequired = true;

        /// <summary>
        /// Current position.
        /// </summary>     
        private int _pos;

        public JBlock()
            : this(true, true)
        {
        }

        public JBlock(bool bracesRequired, bool indentRequired)
        {
            this.bracesRequired = bracesRequired;
            this.indentRequired = indentRequired;
        }

        /// <summary>
        /// Returns a read-only view of <seealso cref="JStatement"/>s and <seealso cref="JDeclaration"/>
        /// in this block.
        /// </summary>
        public IList<object> Contents
        {
            get
            {
                return content;
            }
        }

        private T insert<T>(T statementOrDeclaration)
        {
            content.Insert(_pos, statementOrDeclaration);
            _pos++;
            return statementOrDeclaration;
        }

        /// <summary>
        /// Gets the current position to which new statements will be inserted.
        /// 
        /// For example if the value is 0, newly created instructions will be
        /// inserted at the very beginning of the block.
        /// </summary>
        /// <seealso cref= #pos(int) </seealso>
        public int pos()
        {
            return _pos;
        }

        /// <summary>
        /// Sets the current position.
        /// 
        /// @return
        ///      the old value of the current position. </summary>
        /// <exception cref="IllegalArgumentException">
        ///      if the new position value is illegal.
        /// </exception>
        /// <seealso cref= #pos() </seealso>
        public int pos(int newPos)
        {
            int r = _pos;
            if (newPos > content.Count || newPos < 0)
            {
                throw new System.ArgumentException();
            }
            _pos = newPos;

            return r;
        }

        /// <summary>
        /// Returns true if this block is empty and does not contain
        /// any statement.
        /// </summary>
        public bool isEmpty
        {
            get
            {
                return content.Count == 0;
            }
        }

        /// <summary>
        /// Adds a local variable declaration to this block
        /// </summary>
        /// <param name="type">
        ///        JType of the variable
        /// </param>
        /// <param name="name">
        ///        Name of the variable
        /// </param>
        /// <returns> Newly generated JVar </returns>
        public JVar decl(JType type, string name)
        {
            return decl(JMod.NONE, type, name, null);
        }

        /// <summary>
        /// Adds a local variable declaration to this block
        /// </summary>
        /// <param name="type">
        ///        JType of the variable
        /// </param>
        /// <param name="name">
        ///        Name of the variable
        /// </param>
        /// <param name="init">
        ///        Initialization expression for this variable.  May be null.
        /// </param>
        /// <returns> Newly generated JVar </returns>
        public JVar decl(JType type, string name, JExpression init)
        {
            return decl(JMod.NONE, type, name, init);
        }

        /// <summary>
        /// Adds a local variable declaration to this block
        /// </summary>
        /// <param name="mods">
        ///        Modifiers for the variable
        /// </param>
        /// <param name="type">
        ///        JType of the variable
        /// </param>
        /// <param name="name">
        ///        Name of the variable
        /// </param>
        /// <param name="init">
        ///        Initialization expression for this variable.  May be null.
        /// </param>
        /// <returns> Newly generated JVar </returns>
        public JVar decl(int mods, JType type, string name, JExpression init)
        {
            JVar v = new JVar(JMods.forVar(mods), type, name, init);
            insert(v);
            bracesRequired = true;
            indentRequired = true;
            return v;
        }

        /// <summary>
        /// Creates an assignment statement and adds it to this block.
        /// </summary>
        /// <param name="lhs">
        ///        Assignable variable or field for left hand side of expression
        /// </param>
        /// <param name="exp">
        ///        Right hand side expression </param>
        public JBlock assign(JAssignmentTarget lhs, JExpression exp)
        {
            insert(new JAssignment(lhs, exp));
            return this;
        }

        public JBlock assignPlus(JAssignmentTarget lhs, JExpression exp)
        {
            insert(new JAssignment(lhs, exp, "+"));
            return this;
        }

        /// <summary>
        /// Creates an invocation statement and adds it to this block.
        /// </summary>
        /// <param name="expr">
        ///        JExpression evaluating to the class or object upon which
        ///        the named method will be invoked
        /// </param>
        /// <param name="method">
        ///        Name of method to invoke
        /// </param>
        /// <returns> Newly generated JInvocation </returns>
        public JInvocation invoke(JExpression expr, string method)
        {
            JInvocation i = new JInvocation(expr, method);
            insert(i);
            return i;
        }

        /// <summary>
        /// Creates an invocation statement and adds it to this block.
        /// </summary>
        /// <param name="expr">
        ///        JExpression evaluating to the class or object upon which
        ///        the method will be invoked
        /// </param>
        /// <param name="method">
        ///        JMethod to invoke
        /// </param>
        /// <returns> Newly generated JInvocation </returns>
        public JInvocation invoke(JExpression expr, JMethod method)
        {
            return insert(new JInvocation(expr, method));
        }

        /// <summary>
        /// Creates a static invocation statement.
        /// </summary>
        public JInvocation staticInvoke(JClass type, string method)
        {
            return insert(new JInvocation(type, method));
        }

        /// <summary>
        /// Creates an invocation statement and adds it to this block.
        /// </summary>
        /// <param name="method">
        ///        Name of method to invoke
        /// </param>
        /// <returns> Newly generated JInvocation </returns>
        public JInvocation invoke(string method)
        {
            return insert(new JInvocation((JExpression)null, method));
        }

        /// <summary>
        /// Creates an invocation statement and adds it to this block.
        /// </summary>
        /// <param name="method">
        ///        JMethod to invoke
        /// </param>
        /// <returns> Newly generated JInvocation </returns>
        public JInvocation invoke(JMethod method)
        {
            return insert(new JInvocation((JExpression)null, method));
        }

        /// <summary>
        /// Adds a statement to this block
        /// </summary>
        /// <param name="s">
        ///        JStatement to be added
        /// </param>
        /// <returns> This block </returns>
        public JBlock add(JStatement s)
        { // ## Needed?
            insert(s);
            return this;
        }

        /// <summary>
        /// Create an If statement and add it to this block
        /// </summary>
        /// <param name="expr">
        ///        JExpression to be tested to determine branching
        /// </param>
        /// <returns> Newly generated conditional statement </returns>
        public JConditional _if(JExpression expr)
        {
            return insert(new JConditional(expr));
        }

        /// <summary>
        /// Create a For statement and add it to this block
        /// </summary>
        /// <returns> Newly generated For statement </returns>
        public JForLoop _for()
        {
            return insert(new JForLoop());
        }

        /// <summary>
        /// Create a While statement and add it to this block
        /// </summary>
        /// <returns> Newly generated While statement </returns>
        public JWhileLoop _while(JExpression test)
        {
            return insert(new JWhileLoop(test));
        }

        /// <summary>
        /// Create a switch/case statement and add it to this block
        /// </summary>
        public JSwitch _switch(JExpression test)
        {
            return insert(new JSwitch(test));
        }

        /// <summary>
        /// Create a Do statement and add it to this block
        /// </summary>
        /// <returns> Newly generated Do statement </returns>
        public JDoLoop _do(JExpression test)
        {
            return insert(new JDoLoop(test));
        }

        /// <summary>
        /// Create a Try statement and add it to this block
        /// </summary>
        /// <returns> Newly generated Try statement </returns>
        public JTryBlock _try()
        {
            return insert(new JTryBlock());
        }

        /// <summary>
        /// Create a return statement and add it to this block
        /// </summary>
        public void _return()
        {
            insert(new JReturn(null));
        }

        /// <summary>
        /// Create a return statement and add it to this block
        /// </summary>
        public void _return(JExpression exp)
        {
            insert(new JReturn(exp));
        }

        /// <summary>
        /// Create a throw statement and add it to this block
        /// </summary>
        public void _throw(JExpression exp)
        {
            insert(new JThrow(exp));
        }

        /// <summary>
        /// Create a break statement and add it to this block
        /// </summary>
        public void _break()
        {
            _break(null);
        }

        public void _break(JLabel label)
        {
            insert(new JBreak(label));
        }

        /// <summary>
        /// Create a label, which can be referenced from
        /// <code>continue</code> and <code>break</code> statements.
        /// </summary>
        public JLabel label(string name)
        {
            JLabel l = new JLabel(name);
            insert(l);
            return l;
        }

        /// <summary>
        /// Create a continue statement and add it to this block
        /// </summary>
        public void _continue(JLabel label)
        {
            insert(new JContinue(label));
        }

        public void _continue()
        {
            _continue(null);
        }

        /// <summary>
        /// Create a sub-block and add it to this block
        /// </summary>
        public JBlock block()
        {
            JBlock b = new JBlock();
            b.bracesRequired = false;
            b.indentRequired = false;
            return insert(b);
        }

        /// <summary>
        /// Creates a "literal" statement directly.
        /// 
        /// <para>
        /// Specified string is printed as-is.
        /// This is useful as a short-cut.
        /// 
        /// </para>
        /// <para>
        /// For example, you can invoke this method as:
        /// <code>directStatement("a=b+c;")</code>.
        /// </para>
        /// </summary>     
        public JStatement directStatement(string source)
        {
            JStatement s = new JStatementAnonymousInnerClass(this, source);
            add(s);
            return s;
        }

        private class JStatementAnonymousInnerClass : JStatement
        {
            private readonly JBlock outerInstance;

            private string source;

            public JStatementAnonymousInnerClass(JBlock outerInstance, string source)
            {
                this.outerInstance = outerInstance;
                this.source = source;
            }

            public virtual void state(JFormatter f)
            {
                f.p(source).nl();
            }
        }

        public void generate(JFormatter f)
        {
            if (bracesRequired)
            {
                f.p('{').nl();
            }
            if (indentRequired)
            {
                f.i();
            }
            generateBody(f);
            if (indentRequired)
            {
                f.o();
            }
            if (bracesRequired)
            {
                f.p('}');
            }
        }

        internal void generateBody(JFormatter f)
        {
            foreach (object o in content)
            {
                if (o is JDeclaration)
                {
                    f.d((JDeclaration)o);
                }
                else
                {
                    f.s((JStatement)o);
                }
            }
        }

        /// <summary>
        /// Creates an enhanced For statement based on j2se 1.5 JLS
        /// and add it to this block
        /// </summary>
        /// <returns> Newly generated enhanced For statement per j2se 1.5
        /// specification </returns>
        public JForEach forEach(JType varType, string name, JExpression collection)
        {
            return insert(new JForEach(varType, name, collection));

        }
        public void state(JFormatter f)
        {
            f.g(this);
            if (bracesRequired)
            {
                f.nl();
            }
        }

    }
}
