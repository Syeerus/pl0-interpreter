/**
 * Class for the literal node.
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
    /// Literal node of an AST.
    /// </summary>
    public class LiteralNode : Node
    {
        /// <summary>
        /// Type of the literal.
        /// </summary>
        public readonly DataType Type;

        /// <summary>
        /// Value of the node stored as a string.
        /// </summary>
        public string Value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset in the source string.</param>
        /// <param name="line">Starting line in the source string.</param>
        /// <param name="column">Starting column in the source string.</param>
        /// <param name="type">Type of the literal.</param>
        /// <param name="val">String value of the literal.</param>
        public LiteralNode(int offset, int line, int column, DataType type, string val) : base(offset, line, column)
        {
            Type = type;
            Value = val;
        }

        /// <summary>
        /// Overridden for debugging purposes.
        /// </summary>
        /// <returns>The node converted to a string.</returns>
        public override string ToString()
        {
            return $"[{GetType().Name}, off: {Offset}, line: {Line}, col: {Column}, type: {Type}, val: \"{Value}\"]";
        }
    }
}
