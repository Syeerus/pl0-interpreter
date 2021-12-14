/**
 * Main interpreter.
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

using System;
using System.Collections.Generic;
using Interpreter.Errors;
using Interpreter.Parser.AST;

namespace Interpreter
{
    /// <summary>
    /// The main interpreter.
    /// </summary>
    public class Evaluator
    {
        public bool IsDebugMode = false;

        /// <summary>
        /// Scope stack.
        /// </summary>
        private readonly Stack<Scope> _scopes = new Stack<Scope>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Evaluator()
        {
        }

        /// <summary>
        /// Parses and runs a source code string.
        /// </summary>
        /// <param name="source"></param>
        /// <exception cref="SyntaxError">Raised when a syntax error occurs.</exception>
        /// <exception cref="UnterminatedStringError">Raised when an unterminated string is encountered.</exception>
        /// <exception cref="NameError">Raised when a name is not found.</exception>
        /// <exception cref="RedeclareError">Raised when trying to define a name twice in the same scope.</exception>
        /// <exception cref="ReassignConstantError">Raised when trying to reassign a constant.</exception>
        /// <exception cref="DivideByZeroError">Raised when trying to divide by zero.</exception>
        /// <exception cref="UnsupportedDataTypeError">Raised when trying to assign an unsupported data type.</exception>
        /// <exception cref="UnrecognizedNodeError">Raised when an unrecognized node is encountered.</exception>
        public void Run(string source)
        {
            var parser = new Parser.Parser(source);
            Run(parser.Parse());
        }

        /// <summary>
        /// Runs a program tree.
        /// </summary>
        /// <param name="program">The program to run.</param>
        /// <exception cref="NameError">Raised when a name is not found.</exception>
        /// <exception cref="RedeclareError">Raised when trying to define a name twice in the same scope.</exception>
        /// <exception cref="ReassignConstantError">Raised when trying to reassign a constant.</exception>
        /// <exception cref="DivideByZeroError">Raised when trying to divide by zero.</exception>
        /// <exception cref="UnsupportedDataTypeError">Raised when trying to assign an unsupported data type.</exception>
        /// <exception cref="UnrecognizedNodeError">Raised when an unrecognized node is encountered.</exception>
        public void Run(ProgramNode program)
        {
            if (IsDebugMode)
            {
                Console.WriteLine(program);
            }

            RunBlockNode(program.Body);
        }

        /// <summary>
        /// Performs an integer binary operation.
        /// </summary>
        /// <param name="left">Left integer.</param>
        /// <param name="right">Right integer.</param>
        /// <param name="op">Operator.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="DivideByZeroError">Thrown when trying to divide by zero.</exception>
        /// <exception cref="OperatorError">Thrown on an invalid binary operator.</exception>
        private static int DoIntBinOp(int left, int right, BinaryOperator op)
        {
            switch (op)
            {
                case BinaryOperator.Plus:
                    return left + right;
                case BinaryOperator.Minus:
                    return left - right;
                case BinaryOperator.Star:
                    return left * right;
                case BinaryOperator.Slash:
                    if (right == 0)
                    {
                        throw new DivideByZeroError("Cannot divide by zero.");
                    }
                    return left / right;
                default:
                    throw new OperatorError($"Unexpected binary operator: {op}");
            }
        }

        /// <summary>
        /// Performs a float binary operation.
        /// </summary>
        /// <param name="left">Left float.</param>
        /// <param name="right">Right float.</param>
        /// <param name="op">Operator.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="DivideByZeroError">Thrown when trying to divide by zero.</exception>
        /// <exception cref="OperatorError">Thrown on an invalid binary operator.</exception>
        private static float DoFloatBinOp(float left, float right, BinaryOperator op)
        {
            switch (op)
            {
                case BinaryOperator.Plus:
                    return left + right;
                case BinaryOperator.Minus:
                    return left - right;
                case BinaryOperator.Star:
                    return left * right;
                case BinaryOperator.Slash:
                    if (right == 0.0f)
                    {
                        throw new DivideByZeroError("Cannot divide by zero.");
                    }
                    return left / right;
                default:
                    throw new OperatorError($"Unexpected binary operator: {op}");
            }
        }

        /// <summary>
        /// Creates a new scope.
        /// </summary>
        /// <returns>The scope that was created.</returns>
        private Scope CreateScope()
        {
            var scope = new Scope();
            _scopes.Push(scope);
            return scope;
        }

        /// <summary>
        /// Pops the top of the scope stack, except for the root scope.
        /// </summary>
        private void PopScope()
        {
            if (_scopes.Count > 1)
            {
                _scopes.Pop();
            }
        }

        /// <summary>
        /// Runs a block node.
        /// </summary>
        /// <param name="node">The block to run.</param>
        /// <exception cref="NameError">Raised when a name is not found.</exception>
        /// <exception cref="RedeclareError">Raised when trying to define a name twice in the same scope.</exception>
        /// <exception cref="ReassignConstantError">Raised when trying to reassign a constant.</exception>
        /// <exception cref="DivideByZeroError">Raised when trying to divide by zero.</exception>
        /// <exception cref="UnsupportedDataTypeError">Raised when trying to assign an unsupported data type.</exception>
        /// <exception cref="UnrecognizedNodeError">Raised when an unrecognized node is encountered.</exception>
        private void RunBlockNode(BlockStatementNode node)
        {
            if (node == null)
            {
                return;
            }

            CreateScope();
            foreach (Node n in node.Body)
            {
                RunNode(n);
            }

            if (_scopes.Count > 1)
            {
                // Don't destroy the first scope.
                PopScope();
            }
        }

        /// <summary>
        /// Runs a node.
        /// </summary>
        /// <param name="node">Node to run.</param>
        /// <exception cref="NameError">Raised when a name is not found.</exception>
        /// <exception cref="RedeclareError">Raised when trying to define a name twice in the same scope.</exception>
        /// <exception cref="ReassignConstantError">Raised when trying to reassign a constant.</exception>
        /// <exception cref="DivideByZeroError">Raised when trying to divide by zero.</exception>
        /// <exception cref="UnsupportedDataTypeError">Raised when trying to assign an unsupported data type.</exception>
        /// <exception cref="UnrecognizedNodeError">Raised when an unrecognized node is encountered.</exception>
        private void RunNode(Node node)
        {
            Type type = node.GetType();
            if (type == typeof(VariableDeclarationsNode))
            {
                RunVarDeclarations((VariableDeclarationsNode)node);
            }
            else if (type == typeof(ProcedureDeclarationNode))
            {
                RunProcedureDeclaration((ProcedureDeclarationNode)node);
            }
            else if (type == typeof(AssignmentStatementNode))
            {
                RunAssignmentNode((AssignmentStatementNode)node);
            }
            else if (type == typeof(CallStatementNode))
            {
                RunCallNode((CallStatementNode)node);
            }
            else if (type == typeof(InputStatementNode))
            {
                RunInputNode((InputStatementNode)node);
            }
            else if (type == typeof(PrintStatementNode))
            {
                RunPrintNode((PrintStatementNode)node);
            }
            else if (type == typeof(BeginStatementNode))
            {
                RunBeginNode((BeginStatementNode)node);
            }
            else if (type == typeof(IfStatementNode))
            {
                RunIfNode((IfStatementNode)node);
            }
            else if (type == typeof(WhileStatementNode))
            {
                RunWhileNode((WhileStatementNode)node);
            }
            else if (type == typeof(BlockStatementNode))
            {
                RunBlockNode((BlockStatementNode)node);
            }
            else if (type == typeof(ExitNode))
            {
                // The end.
            }
            else
            {
                throw new UnrecognizedNodeError(node.Line, node.Column, $"Node '{type.ToString()}' not recognized.");
            }
        }

        /// <summary>
        /// Runs a variable declarations node.
        /// </summary>
        /// <param name="node">The variable declarations node.</param>
        private void RunVarDeclarations(VariableDeclarationsNode node)
        {
            Scope scope = _scopes.Peek();
            foreach (VariableDeclarationNode n in node.Declarations)
            {
                if (n.IsConstant)
                {
                    TypedValue value = RunExpressionNode(n.Value);
                    scope.CreateVar(n.Name, value.Value, true);
                }
                else
                {
                    scope.CreateVar(n.Name);
                }
            }
        }

        /// <summary>
        /// Runs a procedure declaration node.
        /// </summary>
        /// <param name="node">The procedure declaration node.</param>
        private void RunProcedureDeclaration(ProcedureDeclarationNode node)
        {
            Scope scope = _scopes.Peek();
            scope.CreateProcedure(node.Name, node);
        }

        /// <summary>
        /// Runs an assignment node.
        /// </summary>
        /// <param name="node">The assignment node.</param>
        private void RunAssignmentNode(AssignmentStatementNode node)
        {
            object value = RunExpressionNode(node.Value);
            Scope scope = _scopes.Peek();
            scope.SetVar(node.Identifier.Name, value);
        }

        /// <summary>
        /// Runs an expression node.
        /// </summary>
        /// <param name="node">The expression node.</param>
        /// <returns>Result of the expression.</returns>
        /// <exception cref="DivideByZeroError">Thrown when attempting to divide by zero.</exception>
        /// <exception cref="OperatorError">Thrown when an unexpected operator is encountered.</exception>
        /// <exception cref="TypeError">Thrown when operating or casting on incompatible types.</exception>
        /// <exception cref="UnrecognizedNodeError">Thrown when an unrecognized node is encountered.</exception>
        private TypedValue RunExpressionNode(Node node)
        {
            TypedValue value;
            Type type = node.GetType();
            if (type == typeof(UnaryExpressionNode))
            {
                value = RunUnaryNode((UnaryExpressionNode)node);
            }
            else if (type == typeof(BinaryExpressionNode))
            {
                value = RunBinaryNode((BinaryExpressionNode)node);
            }
            else if (type == typeof(LiteralNode))
            {
                var literal = (LiteralNode)node;
                switch (literal.Type)
                {
                    case DataType.Integer:
                        value = new TypedValue(int.Parse(literal.Value));
                        break;
                    case DataType.Float:
                        value = new TypedValue(float.Parse(literal.Value));
                        break;
                    case DataType.String:
                        value = new TypedValue(literal.Value);
                        break;
                    default:
                        throw new TypeError(node.Line, node.Column, $"Invalid literal type: {literal.Type}");
                }
            }
            else if (type == typeof(TypecastNode))
            {
                var cast = (TypecastNode)node;
                value = RunExpressionNode(cast.Expression);
                switch (cast.CastType)
                {
                    case DataType.Integer:
                        switch (value.Type)
                        {
                            case DataType.Integer:
                                break;
                            case DataType.Float:
                                value.Value = (int)((float)value.Value);
                                break;
                            case DataType.String:
                            {
                                int temp = 0;
                                int.TryParse((string)value.Value, out temp);
                                value.Value = temp;
                                break;
                            }
                            default:
                                throw new TypeError(node.Line, node.Column, $"Cannot cast {value.Type} to {cast.CastType}");
                        }
                        break;
                    case DataType.Float:
                        switch (value.Type)
                        {
                            case DataType.Integer:
                                value.Value = (float)((int)value.Value);
                                break;
                            case DataType.Float:
                                break;
                            case DataType.String:
                            {
                                float temp = 0.0f;
                                float.TryParse((string)value.Value, out temp);
                                value.Value = temp;
                                break;
                            }
                            default:
                                throw new TypeError(node.Line, node.Column, $"Cannot cast {value.Type} to {cast.CastType}"); 
                        }
                        break;
                    case DataType.String:
                        value.Value = value.Value.ToString();
                        break;
                    default:
                        throw new TypeError(node.Line, node.Column, $"Unsupported cast type: {cast.CastType}");
                }
            }
            else if (type == typeof(IdentifierNode))
            {
                var identifier = (IdentifierNode)node;
                Scope scope = _scopes.Peek();
                value = scope.GetVar(identifier.Name);
            }
            else
            {
                throw new UnrecognizedNodeError(node.Line, node.Column, $"Invalid node expression: {type}");
            }

            return value;
        }

        /// <summary>
        /// Runs a unary node.
        /// </summary>
        /// <param name="node">The unary node.</param>
        /// <returns>Result of the unary expression.</returns>
        /// <exception cref="DivideByZeroError">Thrown when attempting to divide by zero.</exception>
        /// <exception cref="OperatorError">Thrown when an unexpected operator is encountered.</exception>
        /// <exception cref="TypeError">Thrown when operating or casting on incompatible types.</exception>
        /// <exception cref="UnrecognizedNodeError">Thrown when an unrecognized node is encountered.</exception>
        private TypedValue RunUnaryNode(UnaryExpressionNode node)
        {
            TypedValue value = RunExpressionNode(node.Expression);
            if (node.Operator == UnaryOperator.Plus)
            {
                return value;
            }

            switch (value.Type)
            {
                case DataType.Integer:
                    value.Value = (-(int)value.Value);
                    break;
                case DataType.Float:
                    value.Value = (-(float)value.Value);
                    break;
                default:
                    throw new TypeError(node.Line, node.Column, $"Invalid type with unary operator: {value.Type}");
            }

            return value;
        }

        /// <summary>
        /// Runs a binary node.
        /// </summary>
        /// <param name="node">The binary node.</param>
        /// <returns>Result of the binary expression.</returns>
        /// <exception cref="DivideByZeroError">Thrown when attempting to divide by zero.</exception>
        /// <exception cref="OperatorError">Thrown when an unexpected operator is encountered.</exception>
        /// <exception cref="TypeError">Thrown when operating on incompatible types.</exception>
        /// <exception cref="UnrecognizedNodeError">Thrown when an unrecognized node is encountered.</exception>
        private TypedValue RunBinaryNode(BinaryExpressionNode node)
        {
            TypedValue left = RunExpressionNode(node.Left);
            TypedValue right = RunExpressionNode(node.Right);
            switch (left.Type)
            {
                case DataType.Integer:
                    switch (right.Type)
                    {
                        case DataType.Integer:
                            try
                            {
                                left.Value = DoIntBinOp((int)left.Value, (int)right.Value, node.Operator);
                            }
                            catch (RuntimeError ex)
                            {
                                // Update with position information.
                                ex.Line = node.Right.Line;
                                ex.Column = node.Right.Column;
                                throw ex;
                            }
                            break;
                        case DataType.Float:
                            try
                            {
                                left.Value = DoFloatBinOp((float)left.Value, (float)right.Value, node.Operator);
                            }
                            catch (RuntimeError ex)
                            {
                                // Update with position information.
                                ex.Line = node.Right.Line;
                                ex.Column = node.Right.Column;
                                throw ex;
                            }
                            break;
                        default:
                            throw new TypeError(node.Right.Line, node.Right.Column, $"Incompatible data type for binary operator: {right.Type}");
                    }
                    break;
                case DataType.Float:
                    switch (right.Type)
                    {
                        case DataType.Integer:
                        case DataType.Float:
                            try
                            {
                                left.Value = DoFloatBinOp((float)left.Value, (float)right.Value, node.Operator);
                            }
                            catch (DivideByZeroError ex)
                            {
                                // Update with position information.
                                ex.Line = node.Right.Line;
                                ex.Column = node.Right.Column;
                                throw ex;
                            }
                            break;
                        default:
                            throw new TypeError(node.Right.Line, node.Right.Column, $"Incompatible data type for binary operator: {right.Type}");
                    }
                    break;
                case DataType.String:
                    switch (right.Type)
                    {
                        case DataType.String:
                            left.Value = (string)left.Value + (string)right.Value;
                            break;
                    }
                    break;
                default:
                    throw new TypeError(node.Left.Line, node.Left.Column, $"Incompatible data type for binary operator: {left.Type}");
            }

            return left;
        }

        /// <summary>
        /// Runs a call statement node.
        /// </summary>
        /// <param name="node">The call statement node.</param>
        /// <exception cref="NameError">Raised when a name is not found.</exception>
        /// <exception cref="RedeclareError">Raised when trying to define a name twice in the same scope.</exception>
        /// <exception cref="ReassignConstantError">Raised when trying to reassign a constant.</exception>
        /// <exception cref="DivideByZeroError">Raised when trying to divide by zero.</exception>
        /// <exception cref="UnsupportedDataTypeError">Raised when trying to assign an unsupported data type.</exception>
        /// <exception cref="UnrecognizedNodeError">Raised when an unrecognized node is encountered.</exception>
        private void RunCallNode(CallStatementNode node)
        {
            Scope scope = _scopes.Peek();
            ProcedureDeclarationNode procedure = scope.GetProcedure(node.Identifier.Name);
            RunNode(procedure.Body);
        }

        /// <summary>
        /// Runs an input statement node.
        /// </summary>
        /// <param name="node">The input statement node.</param>
        private void RunInputNode(InputStatementNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs a print statement node.
        /// </summary>
        /// <param name="node">The print statement node.</param>
        private void RunPrintNode(PrintStatementNode node)
        {
            TypedValue value = RunExpressionNode(node.Expression);
            Console.WriteLine(value.Value);
        }

        /// <summary>
        /// Runs a begin statement node.
        /// </summary>
        /// <param name="node">The begin statement node.</param>
        private void RunBeginNode(BeginStatementNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs an if statement node.
        /// </summary>
        /// <param name="node">The if statement node.</param>
        private void RunIfNode(IfStatementNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs a while statement node.
        /// </summary>
        /// <param name="node">The while statement node.</param>
        private void RunWhileNode(WhileStatementNode node)
        {
            throw new NotImplementedException();
        }
    }
}
