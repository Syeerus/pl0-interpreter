/**
 * Parser/Lexer errors.
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
    /// Base parser error class.
    /// </summary>
    public abstract class BaseError : Exception
    {
        /// <summary>
        /// Line position of the error.
        /// </summary>
        public readonly int Line;

        /// <summary>
        /// Column position of the error.
        /// </summary>
        public readonly int Column;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position of the error.</param>
        /// <param name="column">Column position of the error.</param>
        /// <param name="message">Error message.</param>
        public BaseError(int line, int column, string message) : base(message)
        {
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Converts the error into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"( {Line}, {Column} ) {Message}";
        }
    }

    /// <summary>
    /// Error for an unterminated string.
    /// </summary>
    public class UnterminatedStringError : BaseError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position of the error.</param>
        /// <param name="column">Column position of the error.</param>
        /// <param name="message">Error message.</param>
        public UnterminatedStringError(int line, int column, string message) : base(line, column, message) { }
    }
}
