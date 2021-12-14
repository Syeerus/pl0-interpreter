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
using Interpreter.Errors;

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

            if (Matches(TokenType.Semicolon))
            {
                // Trailing semicolon.
                Advance();
            }
            else if (Matches(TokenType.Dot))
            {
                // End of program.
                // Don't advance because Parse() asserts the dot.
                t = _currentToken;
                block.Body.Add(new ExitNode(t.Offset, t.Line, t.Column));
            }

            return block;
        }

        /// <summary>
        /// Checks if a token is an unary operator.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>If the token is an unary operator.</returns>
        private static bool IsUnaryOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a token is a binary operator.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>If the token is a binary operator.</returns>
        private static bool IsBinaryOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Star:
                case TokenType.Slash:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a token is a conditional operator.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>If the token is a conditional operator.</returns>
        private static bool IsConditionalOperator(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Equals:
                case TokenType.Hash:
                case TokenType.LessThan:
                case TokenType.GreaterThan:
                case TokenType.LessThanEquals:
                case TokenType.GreaterThanEquals:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a token is a literal.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>If the token is a literal.</returns>
        private static bool IsLiteral(Token token)
        {
            switch (token.Type)
            {
                case TokenType.IntegerLiteral:
                case TokenType.FloatLiteral:
                case TokenType.StringLiteral:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the data type by keyword token.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>A corresponding data type.</returns>
        private static DataType GetDataType(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Int:
                    return DataType.Integer;
                case TokenType.Float:
                    return DataType.Float;
                case TokenType.String:
                    return DataType.String;
            }

            return DataType.Invalid;
        }

        /// <summary>
        /// Gets the data type of a literal token.
        /// </summary>
        /// <param name="token">Token to check.</param>
        /// <returns>A corresponding data type.</returns>
        private static DataType GetLiteralDataType(Token token)
        {
            switch (token.Type)
            {
                case TokenType.IntegerLiteral:
                    return DataType.Integer;
                case TokenType.FloatLiteral:
                    return DataType.Float;
                case TokenType.StringLiteral:
                    return DataType.String;
            }

            return DataType.Invalid;
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

            Token t = _currentToken;
            if (Matches(TokenType.Identifier))
            {
                var stmt = new AssignmentStatementNode(t.Offset, t.Line, t.Column);
                stmt.Identifier = new IdentifierNode(t.Offset, t.Line, t.Column);
                stmt.Identifier.Name = t.Value;
                Advance();
                AssertMatches(TokenType.ColonEquals);
                Advance();
                stmt.Value = ParseExpression();
                return stmt;
            }
            else if (Matches(TokenType.Call))
            {
                var stmt = new CallStatementNode(t.Offset, t.Line, t.Column);
                t = Advance();
                AssertMatches(TokenType.Identifier);
                stmt.Identifier = new IdentifierNode(t.Offset, t.Line, t.Column);
                stmt.Identifier.Name = t.Value;
                Advance();
                return stmt;
            }
            else if (Matches(TokenType.QuestionMark))
            {
                var stmt = new InputStatementNode(t.Offset, t.Line, t.Column);
                t = Advance();
                AssertMatches(TokenType.Identifier);
                stmt.Identifier = new IdentifierNode(t.Offset, t.Line, t.Column);
                stmt.Identifier.Name = t.Value;
                Advance();
                return stmt;
            }
            else if (Matches(TokenType.ExclamationMark))
            {
                var stmt = new PrintStatementNode(t.Offset, t.Line, t.Column);
                Advance();
                stmt.Expression = ParseExpression();
                return stmt;
            }
            else if (Matches(TokenType.Begin))
            {
                var stmt = new BeginStatementNode(t.Offset, t.Line, t.Column);
                do
                {
                    Advance();
                    Node n = ParseStatement();
                    if (n != null)
                    {
                        // Don't add blank statements.
                        stmt.Body.Add(n);
                    }

                    if (Matches(TokenType.End))
                    {
                        Advance();
                        break;
                    }
                } while (Matches(TokenType.Semicolon));

                return stmt;
            }
            else if (Matches(TokenType.If))
            {
                var stmt = new IfStatementNode(t.Offset, t.Line, t.Column);
                Advance();
                stmt.Condition = ParseCondition();
                AssertMatches(TokenType.Then);
                Advance();
                stmt.Body = ParseStatement();
                return stmt;
            }
            else if (Matches(TokenType.While))
            {
                var stmt = new WhileStatementNode(t.Offset, t.Line, t.Column);
                Advance();
                stmt.Condition = ParseCondition();
                AssertMatches(TokenType.Do);
                Advance();
                stmt.Body = ParseStatement();
                return stmt;
            }

            throw new SyntaxError(t.Line, t.Column, $"Unrecognized statement: {t.Type}");
        }

        /// <summary>
        /// Parses a condition.
        /// </summary>
        /// <returns>An AST condition node.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private ConditionNode ParseCondition()
        {
            Token t = _currentToken;
            var condition = new ConditionNode(t.Offset, t.Line, t.Column);
            if (Matches(TokenType.Odd))
            {
                condition.HasOdd = true;
                Advance();
                condition.Left = ParseExpression();
            }
            else
            {
                condition.Left = ParseExpression();
                t = _currentToken;
                if (!IsConditionalOperator(t))
                {
                    throw new SyntaxError(t.Line, t.Column, "Expected conditional operator.");
                }

                condition.Operator = (ConditionalOperator)t.Type;
                Advance();
                condition.Right = ParseExpression();
            }

            return condition;
        }

        /// <summary>
        /// Parses an expression.
        /// </summary>
        /// <returns>An AST expression node.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private Node ParseExpression()
        {
            Node left = null;
            Token t = _currentToken;
            if (IsUnaryOperator(t))
            {
                var unary = new UnaryExpressionNode(t.Offset, t.Line, t.Column);
                unary.Operator = (UnaryOperator)t.Type;
                Advance();
                if (IsUnaryOperator(_currentToken))
                {
                    unary.Expression = ParseExpression();
                    return unary;
                }

                unary.Expression = ParseTerm();
                left = unary;
            }
            else
            {
                left = ParseTerm();
            }

            if (IsUnaryOperator(_currentToken))
            {
                var binary = new BinaryExpressionNode(t.Offset, t.Line, t.Column);
                t = _currentToken;
                binary.Left = left;
                binary.Operator = (BinaryOperator)t.Type;
                Advance();
                binary.Right = ParseExpression();
                return binary;
            }
            else
            {
                return left;
            }

            throw new SyntaxError(t.Line, t.Column, "Expected expression.");
        }

        /// <summary>
        /// Parses a term of the grammar.
        /// </summary>
        /// <returns>A node representing a term.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private Node ParseTerm()
        {
            Token t = _currentToken;
            Node left = ParseFactor();
            if (IsBinaryOperator(_currentToken))
            {
                var binary = new BinaryExpressionNode(t.Offset, t.Line, t.Column);
                t = _currentToken;
                binary.Left = left;
                binary.Operator = (BinaryOperator)t.Type;
                Advance();
                binary.Right = ParseTerm();
                return binary;
            }

            return left;
        }

        /// <summary>
        /// Parses a factor of the grammar.
        /// </summary>
        /// <returns>A node representing a factor.</returns>
        /// <exception cref="SyntaxError">Thrown on a syntax error.</exception>
        /// <exception cref="UnterminatedStringError">Thrown if an unterminated string is encountered.</exception>
        private Node ParseFactor()
        {
            Token t = _currentToken;
            if (t.Type == TokenType.Identifier)
            {
                var ident = new IdentifierNode(t.Offset, t.Line, t.Column);
                ident.Name = t.Value;
                Advance();
                return ident;
            }
            else if (IsLiteral(t))
            {
                var literal = new LiteralNode(t.Offset, t.Line, t.Column, GetLiteralDataType(t), t.Value);
                Advance();
                return literal;
            }
            else if (t.Type == TokenType.LeftParenthesis)
            {
                Advance();
                DataType type = GetDataType(_currentToken);
                if (type != DataType.Invalid)
                {
                    var cast = new TypecastNode(t.Offset, t.Line, t.Column);
                    Advance();
                    AssertMatches(TokenType.RightParenthesis);
                    Advance();
                    cast.CastType = type;
                    cast.Expression = ParseFactor();
                    return cast;
                }

                Node node = ParseExpression();
                AssertMatches(TokenType.RightParenthesis);
                Advance();
                return node;
            }

            throw new SyntaxError(t.Line, t.Column, $"Expected identifier, literal, or expression, but got {t.Type}.");
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
                    case TokenType.IntegerLiteral:
                        constant.Value = new LiteralNode(t.Offset, t.Line, t.Column, DataType.Integer, t.Value);
                        break;
                    case TokenType.FloatLiteral:
                        constant.Value = new LiteralNode(t.Offset, t.Line, t.Column, DataType.Float, t.Value);
                        break;
                    case TokenType.StringLiteral:
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
            Advance();
            AssertMatches(TokenType.Semicolon);
            Advance();
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
            //Console.WriteLine("Advancing " + _currentToken);
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
