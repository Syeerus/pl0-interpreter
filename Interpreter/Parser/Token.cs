/**
 * Token class and types.
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

namespace Interpreter.Parser
{
    /// <summary>
    /// Types of tokens spit out by the scanner.
    /// </summary>
    public enum TokenType
    {
        Invalid,
        Dot,                // .
        Comma,              // ,
        Semicolon,          // ;
        QuestionMark,       // ?
        ExclamationMark,    // !
        Equals,             // =
        ColonEquals,        // :=
        Hash,               // #
        LessThan,           // <
        GreaterThan,        // >
        LessThanEquals,     // <=
        GreaterThanEquals,  // >=
        Plus,               // +
        Minus,              // -
        Star,               // *
        Slash,              // /
        LeftParenthesis,    // (
        RightParenthesis,   // )
        Identifier,
        IntegerLiteral,
        FloatLiteral,
        StringLiteral,
        EndOfSource,
        // Keywords
        Const,
        Var,
        Procedure,
        Call,
        Begin,
        End,
        If,
        Then,
        Odd,
        While,
        Do,
        Int,
        Float,
        String,
        Write,
        Read
    }

    /// <summary>
    /// Token class.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Starting offset position in the source string.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Starting line number in the source string.
        /// </summary>
        public readonly int Line;

        /// <summary>
        /// Starting column position in the source string.
        /// </summary>
        public readonly int Column;

        /// <summary>
        /// Type of the token.
        /// </summary>
        public readonly TokenType Type;

        /// <summary>
        /// Literal value of the token.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Token constructor.
        /// </summary>
        /// <param name="type">Type of the token.</param>
        /// <param name="offset">Starting offset position in the source string.</param>
        /// <param name="line">Starting line number in the source string.</param>
        /// <param name="column">Starting column position in the source string.</param>
        /// <param name="value">Literal value of the token.</param>
        public Token(TokenType type, int offset, int line, int column, string value = "")
        {
            Type = type;
            Offset = offset;
            Line = line;
            Column = column;
            Value = value;
        }

        /// <summary>
        /// Determines if tokens are equal.
        /// </summary>
        /// <param name="token">Object to compare with.</param>
        /// <returns>If tokens are equal.</returns>
        public bool Equals(Token token)
        {
            return (
                Type == token.Type &&
                Offset == token.Offset &&
                Line == token.Line &&
                Column == token.Column &&
                Value == token.Value
                );
        }

        /// <summary>
        /// Returns a string representation of the token.
        /// </summary>
        /// <returns>A string representation of the token.</returns>
        public override string ToString()
        {
            return $"[{Type}, off: {Offset}, line: {Line}, col: {Column}, val: \"{Value}\"]";
        }
    }
}
