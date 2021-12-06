/**
 * Class for the unary expression node.
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
    /// Unary operators.
    /// </summary>
    public enum UnaryOperator
    {
        Invalid,
        Plus = TokenType.Plus,
        Minus = TokenType.Minus,
        Odd = TokenType.Odd
    }

    /// <summary>
    /// Unary expression node for an AST.
    /// </summary>
    public class UnaryExpressionNode : Node
    {
        /// <summary>
        /// Operator to use on the expression.
        /// </summary>
        public UnaryOperator Operator;

        /// <summary>
        /// The expression in AST form.
        /// </summary>
        public Node Expression;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset of the source string.</param>
        /// <param name="line">Starting line of the source string.</param>
        /// <param name="column">Starting column of the source string.</param>
        public UnaryExpressionNode(int offset, int line, int column) : base(offset, line, column) { }

        /// <summary>
        /// Overridden for debugging purposes.
        /// </summary>
        /// <returns>The node converted to a string.</returns>
        public override string ToString()
        {
            string output = $"{GetType().Name}, off: {Offset}, line: {Line}, col: {Column}, op: {Operator}]";
            if (Expression != null)
            {
                output += "\nExpression = " + Expression.ToString();
            }

            return output;
        }
    }
}
