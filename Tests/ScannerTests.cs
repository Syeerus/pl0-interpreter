/**
 * Scanner unit tests.
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
using Interpreter.Parser;

namespace Tests
{
    [TestClass]
    public class ScannerTests
    {
        private void TokensAreEqual(Scanner scanner, Token[] expected)
        {
            Console.WriteLine(scanner.Source);
            for (int i = 0; i < expected.Length; ++i)
            {
                Token t = scanner.GetNext();
                Console.WriteLine("Comparing " + t.ToString() + " == " + expected[i].ToString());
                Assert.IsTrue(t.Equals(expected[i]));
            }
        }

        [TestMethod]
        public void IdentifiersTest()
        {
            var scanner = new Scanner("Hello _world a1234");
            Token[] expected =
            {
                new Token(TokenType.Identifier, 0, 1, 1, "Hello"),
                new Token(TokenType.Identifier, 6, 1, 7, "_world"),
                new Token(TokenType.Identifier, 13, 1, 14, "a1234")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void OperatorsTest()
        {
            var scanner = new Scanner("+- */ =# <><=>= := ()");
            Token[] expected =
            {
                new Token(TokenType.Plus, 0, 1, 1),
                new Token(TokenType.Minus, 1, 1, 2),
                new Token(TokenType.Star, 3, 1, 4),
                new Token(TokenType.Slash, 4, 1, 5),
                new Token(TokenType.Equals, 6, 1, 7),
                new Token(TokenType.Hash, 7, 1, 8),
                new Token(TokenType.LessThan, 9, 1, 10),
                new Token(TokenType.GreaterThan, 10, 1, 11),
                new Token(TokenType.LessThanEquals, 11, 1, 12),
                new Token(TokenType.GreaterThanEquals, 13, 1, 14),
                new Token(TokenType.ColonEquals, 16, 1, 17),
                new Token(TokenType.LeftParenthesis, 19, 1, 20)
            };
           TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void OtherTest()
        {
            var scanner = new Scanner(".,;?!");
            Token[] expected =
            {
                new Token(TokenType.Dot, 0, 1, 1),
                new Token(TokenType.Comma, 1, 1, 2),
                new Token(TokenType.Semicolon, 2, 1, 3),
                new Token(TokenType.QuestionMark, 3, 1, 4),
                new Token(TokenType.ExclamationMark, 4, 1, 5)
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void UnterminatedStringTest()
        {
            var scanner = new Scanner("'This string isn\\'t terminated");
            Assert.ThrowsException<UnterminatedStringError>(() => { scanner.GetNext(); });

            scanner = new Scanner("\"This string isn't terminated");
            Assert.ThrowsException<UnterminatedStringError>(() => { scanner.GetNext(); });
        }

        [TestMethod]
        public void BasicStringTest()
        {
            // Single quotes.
            var scanner = new Scanner("'This is a string.'");
            Token[] expected =
            {
                new Token(TokenType.String, 0, 1, 1, "This is a string.")
            };
            TokensAreEqual(scanner, expected);

            // Double quotes.
            scanner = new Scanner("\"This is a string.\"");
            expected = new Token[]
            {
                new Token(TokenType.String, 0, 1, 1, "This is a string.")
            };
            TokensAreEqual(scanner, expected);

            // Multiple strings
            scanner = new Scanner("\"This is a string.\" 'This is another.'");
            expected = new Token[]
            {
                new Token(TokenType.String, 0, 1, 1, "This is a string."),
                new Token(TokenType.String, 20, 1, 21, "This is another.")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void StringNewlineTest()
        {
            var scanner = new Scanner("'Hello\nWorld.'");
            Token[] expected =
            {
                new Token(TokenType.String, 0, 1, 1, "Hello\nWorld.")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void StringEscapeTest()
        {
            var scanner = new Scanner("'Hello\\t\\nworld.'");
            Token[] expected =
            {
                new Token(TokenType.String, 0, 1, 1, "Hello\\t\\nworld.")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void IntegerTest()
        {
            // Decimal
            var scanner = new Scanner("1 23 456");
            Token[] expected =
            {
                new Token(TokenType.Integer, 0, 1, 1, "1"),
                new Token(TokenType.Integer, 2, 1, 3, "23"),
                new Token(TokenType.Integer, 5, 1, 6, "456")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void HexadecimalTest()
        {
            // Hexadecimal
            var scanner = new Scanner("0x1 0xff 0x1abcde");
            Token[] expected =
            {
                new Token(TokenType.Integer, 0, 1, 1, "1"),
                new Token(TokenType.Integer, 4, 1, 5, "255"),
                new Token(TokenType.Integer, 9, 1, 10, "1752286")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void BinaryTest()
        {
            var scanner = new Scanner("0b1 0b11 0b10101");
            Token[] expected =
            {
                new Token(TokenType.Integer, 0, 1, 1, "1"),
                new Token(TokenType.Integer, 4, 1, 5, "3"),
                new Token(TokenType.Integer, 9, 1, 10, "21")
            };
            TokensAreEqual(scanner, expected);

            // Invalid
            scanner = new Scanner("0b2");
            expected = new Token[]
            {
                new Token(TokenType.Integer, 0, 1, 1, "0"),
                new Token(TokenType.Identifier, 1, 1, 2, "b2")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void OctalTest()
        {
            var scanner = new Scanner("01 023 04567");
            Token[] expected =
            {
                new Token(TokenType.Integer, 0, 1, 1, "1"),
                new Token(TokenType.Integer, 3, 1, 4, "19"),
                new Token(TokenType.Integer, 7, 1, 8, "2423")
            };
            TokensAreEqual(scanner, expected);

            // Invalid
            scanner = new Scanner("08");
            expected = new Token[]
            {
                new Token(TokenType.Integer, 0, 1, 1, "0"),
                new Token(TokenType.Integer, 1, 1, 2, "8")
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void FloatTest()
        {
            var scanner = new Scanner("0.1 23.45");
            Token[] expected =
            {
                new Token(TokenType.Float, 0, 1, 1, "0.1"),
                new Token(TokenType.Float, 4, 1, 5, "23.45")
            };
            TokensAreEqual(scanner, expected);

            // Invalid
            scanner = new Scanner("00.2 0.");
            expected = new Token[]
            {
                new Token(TokenType.Integer, 0, 1, 1, "0"),
                new Token(TokenType.Dot, 2, 1, 3),
                new Token(TokenType.Integer, 3, 1, 4, "2"),
            };
            TokensAreEqual(scanner, expected);
            Assert.ThrowsException<MalformedFloatError>(() => { scanner.GetNext(); });
        }

        [TestMethod]
        public void KeywordTest()
        {
            var scanner = new Scanner("const var procedure call begin end if then odd while do");
            Token[] expected =
            {
                new Token(TokenType.Const, 0, 1, 1),
                new Token(TokenType.Var, 6, 1, 7),
                new Token(TokenType.Procedure, 10, 1, 11),
                new Token(TokenType.Call, 20, 1, 21),
                new Token(TokenType.Begin, 25, 1, 26),
                new Token(TokenType.End, 31, 1, 32),
                new Token(TokenType.If, 35, 1, 36),
                new Token(TokenType.Then, 38, 1, 39),
                new Token(TokenType.Odd, 43, 1, 44),
                new Token(TokenType.While, 47, 1, 48),
                new Token(TokenType.Do, 53, 1, 54)
            };
            TokensAreEqual(scanner, expected);

            // Case-insensitive
            scanner = new Scanner("CONST vAr");
            expected = new Token[]
            {
                new Token(TokenType.Const, 0, 1, 1),
                new Token(TokenType.Var, 6, 1, 7)
            };
            TokensAreEqual(scanner, expected);
        }

        [TestMethod]
        public void NewlineTest()
        {
            var scanner = new Scanner("a\nb\n c");
            Token[] expected =
            {
                new Token(TokenType.Identifier, 0, 1, 1, "a"),
                new Token(TokenType.Identifier, 2, 2, 1, "b"),
                new Token(TokenType.Identifier, 5, 3, 2, "c")
            };
            TokensAreEqual(scanner, expected);
        }
    }
}
