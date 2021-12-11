/**
 * Program's scope class.
 * 
 * Copyright (c) 2021 Syeerus
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.Generic;
using Interpreter.Errors;
using Interpreter.Parser.AST;

namespace Interpreter
{
    /// <summary>
    /// A block scope.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Child scope.
        /// </summary>
        public Scope Child;

        /// <summary>
        /// Stored variables.
        /// </summary>
        private readonly Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        /// <summary>
        /// Declared procedures.
        /// </summary>
        private readonly Dictionary<string, ProcedureDeclarationNode> _procedures = new Dictionary<string, ProcedureDeclarationNode>();

        /// <summary>
        /// Parent scope.
        /// </summary>
        private readonly Scope _parent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">Parent scope.</param>
        public Scope(Scope parent = null)
        {
            _parent = parent;
        }

        /// <summary>
        /// Gets a variable by name in this scope or a previous scope.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <returns>A variable in this or a previous scope, or null if none found.</returns>
        public Variable GetVar(string name)
        {
            if (_variables.ContainsKey(name))
            {
                return _variables[name];
            }

            if (_parent != null)
            {
                // Traverse the parent scopes.
                return _parent.GetVar(name);
            }

            return null;
        }

        /// <summary>
        /// Sets the value of a variable in this scope or a previous scope.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Value to store.</param>
        /// <exception cref="NameError">Thrown when no variable by that name is found.</exception>
        /// <exception cref="ReassignConstantError">Thrown when trying to reassign a constant.</exception>
        public void SetVar(string name, object value)
        {
            Variable variable = GetVar(name);
            if (variable == null)
            {
                throw new NameError($"Name '{name}' not found.");
            }

            variable.Value = value;
        }

        /// <summary>
        /// Creates a new scoped variable.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="isConst">If the variable should be marked as constant.</param>
        /// <returns>The variable created.</returns>
        /// <exception cref="RedeclareError">Thrown when trying to redeclare a variable in the same scope.</exception>
        public Variable CreateVar(string name, object value = null, bool isConst = false)
        {
            if (_variables.ContainsKey(name) || _procedures.ContainsKey(name))
            {
                throw new RedeclareError($"Cannot redeclare '{name}' in the same scope.");
            }

            var variable = new Variable(value, isConst);
            _variables.Add(name, variable);
            return variable;
        }

        /// <summary>
        /// Creates a scoped procedure.
        /// </summary>
        /// <param name="name">Name of the procedure.</param>
        /// <param name="procedure">The procedure to store.</param>
        /// <exception cref="RedeclareError">Thrown when trying to redeclare a procedure in the same scope.</exception>
        public void CreateProcedure(string name, ProcedureDeclarationNode procedure)
        {
            if (_procedures.ContainsKey(name) || _variables.ContainsKey(name))
            {
                throw new RedeclareError($"Cannot redeclare '{name}' in the same scope.");
            }

            _procedures.Add(name, procedure);
        }

        /// <summary>
        /// Gets a procedure by name in this scope or a previous scope.
        /// </summary>
        /// <param name="name">Name of the procedure.</param>
        /// <returns>The procedure with that name, or null if not found.</returns>
        public ProcedureDeclarationNode GetProcedure(string name)
        {
            if (_procedures.ContainsKey(name))
            {
                return _procedures[name];
            }

            if (_parent != null)
            {
                // Traverse the parent scopes.
                return _parent.GetProcedure(name);
            }

            return null;
        }
    }
}
