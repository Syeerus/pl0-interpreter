/**
 * Parser unit tests.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interpreter.Parser.AST;
using Interpreter.Parser;
using Interpreter;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void BlankSourceTest()
        {
            var parser = new Parser("");
            Assert.ThrowsException<SyntaxError>(() => { parser.Parse(); });
        }

        [TestMethod]
        public void SmallestProgramTest()
        {
            var parser = new Parser(".");
            ProgramNode program = parser.Parse();
            Console.WriteLine(program);
            var expected = new ProgramNode(0, 1, 1);
            Assert.AreEqual(expected.ToString(), program.ToString());
        }

        [TestMethod]
        public void ConstantsTest()
        {
            var parser = new Parser("const a = 1, b = 2.2, c = 'Hello World';.");
            ProgramNode program = parser.Parse();
            Console.WriteLine("Parsing \"" + parser.Source + "\"");
            Console.WriteLine(program);
        }

        [TestMethod]
        public void InvalidConstantsTest()
        {
            var parser = new Parser("const a, b = 2;.");
            Assert.ThrowsException<SyntaxError>(() => parser.Parse());
        }

        [TestMethod]
        public void VariablesTest()
        {
            var parser = new Parser("var a, b, c;.");
            ProgramNode program = parser.Parse();
            Console.WriteLine("Parsing \"" + parser.Source + "\"");
            Console.WriteLine(program);
        }
    }
}
