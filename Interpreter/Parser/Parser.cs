/**
 * Parser class.
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
using Interpreter.Parser.AST;

namespace Interpreter.Parser
{
    /// <summary>
    /// Parses source code.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Source string.
        /// </summary>
        public readonly string Source;

        /// <summary>
        /// Scanner used for tokenization.
        /// </summary>
        private readonly Scanner _scanner;

        private Token _currentToken;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="source">Source code to parse.</param>
        public Parser(string source)
        {
            Source = source;
            _scanner = new Scanner(source);
        }

        /// <summary>
        /// Parses the program source code.
        /// </summary>
        /// <returns>An AST of the program.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        public ProgramNode Parse()
        {
            Token t = Advance();
            var program = new ProgramNode(t.Offset, t.Line, t.Column);
            program.Body = ParseBlock();
            AssertMatches(TokenType.Dot);
            return program;
        }

        /// <summary>
        /// Parses a block statement.
        /// </summary>
        /// <returns>An AST block statement or null if at the end of the program.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        public BlockStatementNode ParseBlock()
        {
            if (IsAtEnd())
            {
                return null;
            }

            Token t = _currentToken;
            var block = new BlockStatementNode(t.Offset, t.Line, t.Column);
            if (Matches(TokenType.Const))
            {
                block.Body.Add(ParseConstants());
            }
            else if (Matches(TokenType.Var))
            {
                block.Body.Add(ParseVariables());
            }
            else if (Matches(TokenType.Procedure))
            {
                do
                {
                    block.Body.Add(ParseProcedure());
                } while (Matches(TokenType.Procedure));
            }

            Node statement = ParseStatement();
            if (statement != null)
            {
                block.Body.Add(statement);
            }

            return block;
        }

        /// <summary>
        /// Parses a statement.
        /// </summary>
        /// <returns>An AST node of a statement, or null if at the end or is an empty statement.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private Node ParseStatement()
        {
            if (IsAtEnd())
            {
                return null;
            }
            else if (Matches(TokenType.Semicolon))
            {
                // Blank statement.
                Advance();
                return null;
            }

            if (Matches(TokenType.Identifier))
            {
                Advance();
                AssertMatches(TokenType.ColonEquals);
                // TODO
            }
            else if (Matches(TokenType.Call))
            {
                Advance();
                AssertMatches(TokenType.Identifier);
                // TODO
            }
            else if (Matches(TokenType.QuestionMark))
            {
                Advance();
                AssertMatches(TokenType.Identifier);
                // TODO
            }
            else if (Matches(TokenType.ExclamationMark))
            {
                Advance();
                // TODO
            }

            Token t = _currentToken;
            throw new SyntaxError(t.Line, t.Column, $"Unrecognized statement: {t.Type}");
        }

        /// <summary>
        /// Parses constants.
        /// </summary>
        /// <returns>A variable declarations node.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private VariableDeclarationsNode ParseConstants()
        {
            Token t = _currentToken;
            var declarations = new VariableDeclarationsNode(t.Offset, t.Line, t.Column);
            while (true)
            {
                t = Advance();
                AssertMatches(TokenType.Identifier);
                var constant = new VariableDeclarationNode(t.Offset, t.Line, t.Column, true);
                constant.Name = t.Value;
                Advance();
                AssertMatches(TokenType.Equals);
                t = Advance();
                switch (t.Type)
                {
                    case TokenType.Integer:
                        constant.Value = new LiteralNode(t.Offset, t.Line, t.Column, DataType.Integer, t.Value);
                        break;
                    case TokenType.Float:
                        constant.Value = new LiteralNode(t.Offset, t.Line, t.Column, DataType.Float, t.Value);
                        break;
                    case TokenType.String:
                        constant.Value = new LiteralNode(t.Offset, t.Line, t.Column, DataType.String, t.Value);
                        break;
                    default:
                        throw new SyntaxError(t.Line, t.Column, $"Expected an integer, float, or string, but got '{t.Type}'");
                }
                
                declarations.Declarations.Add(constant);
                t = Advance();
                if (Matches(TokenType.Semicolon))
                {
                    break;
                }

                if (!Matches(TokenType.Comma))
                {
                    throw new SyntaxError(t.Line, t.Column, $"Expected comma or semicolon but got '{t.Type}'");
                }
            }

            Advance();
            return declarations;
        }

        /// <summary>
        /// Parses variable declarations.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private VariableDeclarationsNode ParseVariables()
        {
            Token t = _currentToken;
            var declarations = new VariableDeclarationsNode(t.Offset, t.Line, t.Column);
            while (true)
            {
                t = Advance();
                AssertMatches(TokenType.Identifier);
                var variable = new VariableDeclarationNode(t.Offset, t.Line, t.Column);
                variable.Name = t.Value;

                declarations.Declarations.Add(variable);
                t = Advance();
                if (Matches(TokenType.Semicolon))
                {
                    break;
                }

                if (!Matches(TokenType.Comma))
                {
                    throw new SyntaxError(t.Line, t.Offset, $"Expected comma or semicolon but got '{t.Type}'");
                }
            }

            Advance();
            return declarations;
        }

        /// <summary>
        /// Parses a procedure declaration.
        /// </summary>
        /// <returns>Procedure declaration node.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private ProcedureDeclarationNode ParseProcedure()
        {
            Token t = _currentToken;
            var procedure = new ProcedureDeclarationNode(t.Offset, t.Line, t.Column);
            t = Advance();
            AssertMatches(TokenType.Identifier);
            procedure.Name = t.Value;
            AssertMatches(TokenType.Semicolon);
            procedure.Body = ParseBlock();
            return procedure;
        }

        /// <summary>
        /// Advances to the next token.
        /// </summary>
        /// <returns>The next token from the scanner.</returns>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private Token Advance()
        {
            _currentToken = _scanner.GetNext();
            return _currentToken;
        }

        /// <summary>
        /// Determines if the parser is at the end of the program.
        /// </summary>
        /// <returns>True if at the end of the program, otherwise false.</returns>
        private bool IsAtEnd()
        {
            if (_currentToken != null)
            {
                return (
                    _currentToken.Type == TokenType.Dot ||
                    _currentToken.Type == TokenType.EndOfSource
                    );
            }

            return false;
        }

        /// <summary>
        /// Checks if the current token's type matches a given token type.
        /// </summary>
        /// <param name="expectedType">The type to check for.</param>
        /// <returns>True if it matches, otherwise false.</returns>
        private bool Matches(TokenType expectedType)
        {
            return (_currentToken.Type == expectedType);
        }

        /// <summary>
        /// Asserts that the current token's type matches a given token type.
        /// </summary>
        /// <param name="expectedType">The type to check for.</param>
        /// <exception cref="SyntaxError">Thrown if the token type doesn't match.</exception>
        private void AssertMatches(TokenType expectedType)
        {
            if (!Matches(expectedType))
            {
                var t = _currentToken;
                throw new SyntaxError(t.Line, t.Column,
                    $"Expected '{expectedType}' but got '{t.Type}'");
            }
        }
    }
}
