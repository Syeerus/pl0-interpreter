/**
 * Class for the block statement node.
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
using System.Text;

namespace Interpreter.Parser.AST
{
    /// <summary>
    /// Block statement node for an AST.
    /// </summary>
    public class BlockStatementNode : Node
    {
        /// <summary>
        /// The body of the block statement.
        /// </summary>
        public readonly List<Node> Body = new List<Node>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="offset">Starting offset of the source string.</param>
        /// <param name="line">Starting line of the source string.</param>
        /// <param name="column">Starting column of the source string.</param>
        public BlockStatementNode(int offset, int line, int column) : base(offset, line, column) { }

        /// <summary>
        /// Overridden for debugging purposes.
        /// </summary>
        /// <returns>The node converted to a string.</returns>
        public override string ToString()
        {
            var output = new StringBuilder($"{base.ToString()}\nBody ({Body.Count} statements) = ");
            foreach (Node n in Body)
            {
                output.Append($"\n{n.ToString()}");
            }

            return output.ToString();
        }
    }
}
