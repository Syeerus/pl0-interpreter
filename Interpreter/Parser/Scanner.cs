/**
 * Lexical scanning.
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
    /// String tokenizer.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// The source code given to the scanner.
        /// </summary>
        public readonly string Source;

        /// <summary>
        /// Current offset position in the source string.
        /// </summary>
        private int _offset;

        /// <summary>
        /// Current line within the source string.
        /// </summary>
        private int _line;

        /// <summary>
        /// Current column position in the source string.
        /// </summary>
        private int _column;

        /// <summary>
        /// Scanner constructor.
        /// </summary>
        /// <param name="source">Source string.</param>
        public Scanner(string source)
        {
            Source = source;
            _offset = 0;
            Newline();
        }

        /// <summary>
        /// Gets the next token in the source string.
        /// </summary>
        /// <returns>The next token or null if at the end of the source string.</returns>
        /// <exception cref="UnterminatedStringError">Raised when an unterminated string is encountered.</exception>
        public Token GetNext()
        {
            SkipWhiteSpace();
            if (IsAtEnd())
            {
                return null;
            }

            Token token;
            char c = Read();
            if (IsLetter(c) || c == '_')
            {
                token = GetIdentifierOrKeyword();
            }
            else if (IsDigit(c))
            {
                token = GetNumber();
            }
            else if (IsOperator(c))
            {
                token = GetOperator();
            }
            else if (IsString(c))
            {
                token = GetString();
            }
            else if (IsOther(c))
            {
                token = GetOther();
            }
            else
            {
                // Invalid token.
                token = CreateToken(TokenType.Invalid, c.ToString());
                Advance();
            }

            return token;
        }

        /// <summary>
        /// Advances the character offset position.
        /// </summary>
        /// <param name="n">Number of characters to move.</param>
        private void Advance(int n = 1)
        {
            _offset += n;
            _column += n;
        }

        /// <summary>
        /// Gets a character that is at or ahead of the current offset position.
        /// NOTE: Doesn't move the character offset position!
        /// </summary>
        /// <param name="skip">Number of characters to skip.</param>
        /// <returns>A character, or NUL character if at the end of the string or out of bounds.</returns>
        private char Read(int skip = 0)
        {
            int noffset = _offset + skip;
            if (noffset >= Source.Length || noffset < 0)
            {
                return '\0';
            }

            return Source[noffset];
        }

        /// <summary>
        /// Skips white space characters.
        /// </summary>
        private void SkipWhiteSpace()
        {
            bool isLooping = true;
            while (isLooping)
            {
                char c = Read();
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        Advance();
                        break;
                    default:
                        isLooping = false;
                        break;
                }

                if (c == '\n')
                {
                    Newline();
                }
            }
        }

        /// <summary>
        /// Gets an identifier or keyword token. Advances the offset position.
        /// </summary>
        /// <returns>Identifier or keyword token.</returns>
        private Token GetIdentifierOrKeyword()
        {
            int n = 1;        // How many characters to advance.
            char c = Read(n);
            while (IsLetter(c) || IsDigit(c) || c == '_')
            {
                ++n;
                c = Read(n);
            }

            Token token;
            string name = Source.Substring(_offset, n);
            TokenType type = GetKeywordType(name);
            if (type != TokenType.Invalid)
            {
                token = CreateToken(type);
            }
            else
            {
                token = CreateToken(TokenType.Identifier, name);
            }

            Advance(n);
            return token;
        }

        /// <summary>
        /// Gets the corresponding token type of a keyword.
        /// </summary>
        /// <param name="keyword">Keyword to check.</param>
        /// <returns>The corresponding token type of a keyword, or invalid if no match is found.</returns>
        private TokenType GetKeywordType(string keyword)
        {
            switch (keyword.ToLower())
            {
                case "const":
                    return TokenType.Const;
                case "var":
                    return TokenType.Var;
                case "procedure":
                    return TokenType.Procedure;
                case "call":
                    return TokenType.Call;
                case "begin":
                    return TokenType.Begin;
                case "end":
                    return TokenType.End;
                case "if":
                    return TokenType.If;
                case "then":
                    return TokenType.Then;
                case "odd":
                    return TokenType.Odd;
                case "while":
                    return TokenType.While;
                case "do":
                    return TokenType.Do;
            }

            return TokenType.Invalid;
        }

        /// <summary>
        /// Gets a number token. Advances the offset position.
        /// </summary>
        /// <returns>A number token.</returns>
        private Token GetNumber()
        {
            Token token;
            char c = Read();
            if (c == '0')
            {
                char c2 = Read(1);
                if (IsOctal(c2))
                {
                    // Octal
                    string octStr = GetOctalStr();
                    token = CreateToken(TokenType.Integer, Convert.ToInt32(octStr, 8).ToString());
                    Advance(octStr.Length);
                }
                else
                {
                    char c3 = Read(2);
                    if (c2 == 'x' && IsHexadecimal(c3))
                    {
                        // Hexadecimal
                        string hexStr = GetHexadecimalStr();
                        token = CreateToken(TokenType.Integer, Convert.ToInt32(hexStr, 16).ToString());
                        Advance(hexStr.Length);
                    }
                    else if (c2 == 'b' && IsBinary(c3))
                    {
                        // Binary
                        string binStr = GetBinaryStr();
                        token = CreateToken(TokenType.Integer, Convert.ToInt32(binStr, 2).ToString());
                        Advance(binStr.Length + 2);
                    }
                    else if (c2 == '.')
                    {
                        token = GetIntegerOrFloat();
                    }
                    else
                    {
                        // Zero
                        token = CreateToken(TokenType.Integer, "0");
                        Advance();
                    }
                }
            }
            else
            {
                token = GetIntegerOrFloat();
            }

            return token;
        }

        /// <summary>
        /// Gets a hexadecimal number from the source string. Doesn't advance the offset position.
        /// NOTE: Includes prefix.
        /// </summary>
        /// <returns>A hexadecimal number string.</returns>
        private string GetHexadecimalStr()
        {
            int n = 2;     // Offset position.
            char c = Read(n);
            while (IsHexadecimal(c))
            {
                ++n;
                c = Read(n);
            }

            return Source.Substring(_offset, n);
        }

        /// <summary>
        /// Gets a binary number from the source string. Doesn't advance the offset position.
        /// NOTE: Cuts off the prefix.
        /// </summary>
        /// <returns>A binary number string.</returns>
        private string GetBinaryStr()
        {
            int n = 2;      // Offset position.
            char c = Read(n);
            while (IsBinary(c))
            {
                ++n;
                c = Read(n);
            }

            return Source.Substring(_offset + 2, n - 2);
        }

        /// <summary>
        /// Gets an octal number from the source string. Doesn't move the offset position.
        /// NOTE: Includes prefix.
        /// </summary>
        /// <returns>An octal number string.</returns>
        private string GetOctalStr()
        {
            int n = 1;      // Offset position.
            char c = Read(n);
            while (IsOctal(c))
            {
                ++n;
                c = Read(n);
            }

            return Source.Substring(_offset, n);
        }

        /// <summary>
        /// Gets an integer or float token. Advances the offset position.
        /// </summary>
        /// <returns>An integer or float token.</returns>
        private Token GetIntegerOrFloat()
        {
            int n = 1;      // Offset position.
            TokenType type = TokenType.Integer;
            while (!IsAtEnd(n))
            {
                char c = Read(n);
                if (!IsDigit(c))
                {
                    if (c == '.' && type == TokenType.Integer)
                    {
                        type = TokenType.Float;
                    }
                    else
                    {
                        break;
                    }
                }
                ++n;
            }

            string numStr = Source.Substring(_offset, n);
            if (type == TokenType.Float && numStr[numStr.Length - 1] == '.')
            {
                // Assume fractional part of float.
                numStr += '0';
            }

            Token token = CreateToken(type, numStr);
            Advance(n);
            return token;
        }

        /// <summary>
        /// Gets an operator token. Advances the offset position.
        /// </summary>
        /// <returns>An operator token, or invalid token if there's no match.</returns>
        private Token GetOperator()
        {
            int n = 1;    // How many characters to advance.
            Token token = null;
            char c = Read();
            switch (c)
            {
                case '+':
                    token = CreateToken(TokenType.Plus);
                    break;
                case '-':
                    token = CreateToken(TokenType.Minus);
                    break;
                case '*':
                    token = CreateToken(TokenType.Star);
                    break;
                case '/':
                    token = CreateToken(TokenType.Slash);
                    break;
                case '<':
                    c = Read(1);
                    if (c == '=')
                    {
                        ++n;
                        token = CreateToken(TokenType.LessThanEquals);
                    }
                    else
                    {
                        token = CreateToken(TokenType.LessThan);
                    }
                    break;
                case '>':
                    c = Read(1);
                    if (c == '=')
                    {
                        ++n;
                        token = CreateToken(TokenType.GreaterThanEquals);
                    }
                    else
                    {
                        token = CreateToken(TokenType.GreaterThan);
                    }
                    break;
                case '=':
                    token = CreateToken(TokenType.Equals);
                    break;
                case '#':
                    token = CreateToken(TokenType.Hash);
                    break;
                case ':':
                    c = Read(1);
                    if (c == '=')
                    {
                        ++n;
                        token = CreateToken(TokenType.ColonEquals);
                    }
                    break;
                case '(':
                    token = CreateToken(TokenType.LeftParenthesis);
                    break;
                case ')':
                    token = CreateToken(TokenType.RightParenthesis);
                    break;
            }

            if (token == null)
            {
                // Invalid token.
                token = CreateToken(TokenType.Invalid, c.ToString());
            }

            Advance(n);
            return token;
        }

        /// <summary>
        /// Gets a string token. Advances the offset position.
        /// NOTE: Stripes beginning and end quotes.
        /// </summary>
        /// <returns>A string token.</returns>
        /// <exception cref="UnterminatedStringError">Raised when an unterminated string is encountered.</exception>
        private Token GetString()
        {
            int startLine = _line;
            int startColumn = _column;
            char startChar = Read();
            int n = 1;      // How many characters to advance.
            bool isEscaping = false;
            while (!IsAtEnd(n))
            {
                char c = Read(n);
                if (c == '\\' && !isEscaping)
                {
                    isEscaping = true;
                }
                else if (isEscaping)
                {
                    isEscaping = false;
                    if (c == '\n')
                    {
                        Newline();
                    }
                }
                else if (c == '\n')
                {
                    Newline();
                }
                else if (c == startChar)
                {
                    break;
                }
                ++n;
            }

            if (isEscaping || Read(n) != startChar)
            {
                // Unterminated string.
                throw new UnterminatedStringError(_line, _column, "Unterminated string.");
            }

            ++n;        // Skip end quote.

            Token token = CreateToken(TokenType.String, startLine, startColumn, Source.Substring(_offset + 1, n - 2));
            Advance(n);
            return token;
        }

        /// <summary>
        /// Gets one of the other types of tokens. Advances the offset position.
        /// </summary>
        /// <returns>Another type of token.</returns>
        private Token GetOther()
        {
            char c = Read();
            Token token;
            switch (c)
            {
                case '.':
                    token = CreateToken(TokenType.Dot);
                    break;
                case ',':
                    token = CreateToken(TokenType.Comma);
                    break;
                case ';':
                    token = CreateToken(TokenType.Semicolon);
                    break;
                case '?':
                    token = CreateToken(TokenType.QuestionMark);
                    break;
                case '!':
                    token = CreateToken(TokenType.ExclamationMark);
                    break;
                default:
                    token = CreateToken(TokenType.Invalid, c.ToString());
                    break;
            }

            Advance();
            return token;
        }

        /// <summary>
        /// Adds a newline and resets the column position.
        /// </summary>
        private void Newline()
        {
            ++_line;
            _column = 1;
        }

        /// <summary>
        /// Checks if the scanner offset position is at the end of the source string.
        /// </summary>
        /// <param name="n">Number of characters to add to the offset.</param>
        /// <returns>If the scanner offset position is at the end of the source string.</returns>
        private bool IsAtEnd(int n = 0)
        {
            return (_offset + n >= Source.Length);
        }

        /// <summary>
        /// Checks if a given character is a decimal digit.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is a decimal digit.</returns>
        private bool IsDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }

        /// <summary>
        /// Checks if a given character is a letter of the Latin alphabet.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is a letter of the Latin alphabet.</returns>
        private bool IsLetter(char c)
        {
            return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
        }

        /// <summary>
        /// Checks if a given character is a hexadecimal digit.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is a hexadecimal digit.</returns>
        private bool IsHexadecimal(char c)
        {
            char upper = char.ToUpper(c);
            return (char.IsDigit(upper) || (upper >= 'A' && upper <= 'F'));
        }

        /// <summary>
        /// Checks if a given character is a binary digit.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is a binary digit.</returns>
        private bool IsBinary(char c)
        {
            return (c == '0' || c == '1');
        }

        /// <summary>
        /// Checks if a given character is an octal digit.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is an octal digit.</returns>
        private bool IsOctal(char c)
        {
            return (c >= '0' && c <= '7');
        }

        /// <summary>
        /// Checks if a given character is an operator.
        /// NOTE: Will check for certain multi-character operators as well, such as ":=".
        /// </summary>
        /// <param name="c">Character to check.</param>
        /// <returns>If the given character is an operator.</returns>
        private bool IsOperator(char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '<':
                case '>':
                case '=':
                case '#':
                case '(':
                case ')':
                    return true;
                case ':':
                    if (Read(1) == '=')
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// Checks if a given character is the start of a string.
        /// </summary>
        /// <param name="c">Character to test.</param>
        /// <returns>If the given character is the start of a string.</returns>
        private bool IsString(char c)
        {
            return (c == '"' || c == '\'');
        }

        /// <summary>
        /// Checks if a given character is another type of token.
        /// </summary>
        /// <param name="c">Character to check.</param>
        /// <returns>If the given character is another type of token.</returns>
        private bool IsOther(char c)
        {
            switch (c)
            {
                case '.':
                case ',':
                case ';':
                case '?':
                case '!':
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new token with current positions.
        /// </summary>
        /// <param name="type">Type of the token.</param>
        /// <returns>A new token.</returns>
        private Token CreateToken(TokenType type)
        {
            return new Token(type, _offset, _line, _column);
        }

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="type">Type of the token.</param>
        /// <param name="value">Literal value.</param>
        /// <returns>A new token.</returns>
        private Token CreateToken(TokenType type, string value = "")
        {
            return new Token(type, _offset, _line, _column, value);
        }

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="type">Type of the token.</param>
        /// <param name="line">Starting line.</param>
        /// <param name="column">Starting column.</param>
        /// <param name="value">Literal value.</param>
        /// <returns>A new token.</returns>
        private Token CreateToken(TokenType type, int line, int column, string value = "")
        {
            return new Token(type, _offset, line, column, value);
        }
    }
}
