/**
 * Class for the binary expression node.
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

namespace Interpreter.Parser.AST
{
    /// <summary>
    /// Binary operators.
    /// </summary>
    public enum BinaryOperator
    {
        Invalid,
        Plus = TokenType.Plus,
        Minus = TokenType.Minus,
        Star = TokenType.Star,
        Slash = TokenType.Slash,
        Equals = TokenType.Equals,
        Hash = TokenType.Hash,
        LessThan = TokenType.LessThan,
        LessThanEquals = TokenType.LessThanEquals,
        GreaterThan = TokenType.GreaterThan,
        GreaterThanEquals = TokenType.GreaterThanEquals,
    }

    /// <summary>
    /// Binary expression node for an AST.
    /// </summary>
    public class BinaryExpressionNode : Node
    {
        /// <summary>
        /// Left hand side of the expression.
        /// </summary>
        public Node Left;

        /// <summary>
        /// Operator to use.
        /// </summary>
        public BinaryOperator Operator;

        /// <summary>
        /// Right hand side of the expression.
        /// </summary>
        public Node Right;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset of the source string.</param>
        /// <param name="line">Starting line of the source string.</param>
        /// <param name="column">Starting column of the source string.</param>
        public BinaryExpressionNode(int offset, int line, int column) : base(offset, line, column) { }
    }
}
