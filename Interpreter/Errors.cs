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

namespace Interpreter.Errors
{
    /// <summary>
    /// Base parsing error class.
    /// </summary>
    public abstract class ParseError : Exception
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
        public ParseError(int line, int column, string message) : base(message)
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
    public class UnterminatedStringError : ParseError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position of the error.</param>
        /// <param name="column">Column position of the error.</param>
        /// <param name="message">Error message.</param>
        public UnterminatedStringError(int line, int column, string message) : base(line, column, message) { }
    }

    /// <summary>
    /// Error for improper syntax.
    /// </summary>
    public class SyntaxError : ParseError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position of the error.</param>
        /// <param name="column">Column position of the error.</param>
        /// <param name="message">Error message.</param>
        public SyntaxError(int line, int column, string message) : base(line, column, message) { }
    }

    /// <summary>
    /// Base runtime error class.
    /// </summary>
    public abstract class RuntimeError : Exception
    {
        /// <summary>
        /// Line position of the error. Can be set at anytime.
        /// </summary>
        public int Line;

        /// <summary>
        /// Column position of the error. Can be set at anytime.
        /// </summary>
        public int Column;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public RuntimeError(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position in the source code.</param>
        /// <param name="column">Column in the source code.</param>
        /// <param name="message">The message to use.</param>
        public RuntimeError(int line, int column, string message) : base(message)
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
    /// Error for when a variable or procedure is not found at runtime.
    /// </summary>
    public class NameError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public NameError(string message) : base(message) { }
    }

    /// <summary>
    /// Error for when a variable or procedure is redeclared in the same scope.
    /// </summary>
    public class RedeclareError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public RedeclareError(string message) : base(message) { }
    }

    /// <summary>
    /// Error for when trying to divide by zero at runtime.
    /// </summary>
    public class DivideByZeroError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public DivideByZeroError(string message) : base(message) { }
    }

    /// <summary>
    /// Error for when trying to reassign a constant.
    /// </summary>
    public class ReassignConstantError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public ReassignConstantError(string message) : base(message) { }
    }

    /// <summary>
    /// Error for when trying to assign an unsupported data type.
    /// </summary>
    public class UnsupportedDataTypeError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public UnsupportedDataTypeError(string message) : base(message) { }
    }

    /// <summary>
    /// Error for when an unrecognized AST node is encountered.
    /// If this happens, something is seriously wrong.
    /// </summary>
    public class UnrecognizedNodeError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position in the source code.</param>
        /// <param name="column">Column in the source code.</param>
        /// <param name="message">The message to use.</param>
        public UnrecognizedNodeError(int line, int column, string message) : base(line, column, message) { }
    }

    /// <summary>
    /// Error for when working with incompatible data types.
    /// </summary>
    public class TypeError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line position in the source code.</param>
        /// <param name="column">Column in the source code.</param>
        /// <param name="message">The message to use.</param>
        public TypeError(int line, int column, string message) : base(line, column, message) { }
    }

    /// <summary>
    /// Error for unexpected operators.
    /// </summary>
    public class OperatorError : RuntimeError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public OperatorError(string message) : base(message) { }
    }
}
