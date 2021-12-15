/**
 * Main program.
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
using Interpreter.Errors;
using Interpreter.Parser.AST;

namespace Interpreter
{
    /// <summary>
    /// The main program class.
    /// </summary>
    class Interpreter
    {
        /// <summary>
        /// Command line options.
        /// </summary>
        private class Options
        {
            /// <summary>
            /// File name of program to run.
            /// </summary>
            public string FileName;

            /// <summary>
            /// Debug mode option.
            /// </summary>
            public bool IsDebugMode = false;

            /// <summary>
            /// Prompt mode option.
            /// </summary>
            public bool IsPromptMode = false;

            /// <summary>
            /// Show help option.
            /// </summary>
            public bool ShowHelp = false;

            /// <summary>
            /// Show version option.
            /// </summary>
            public bool ShowVersion = false;
        }

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        public static void Main(string[] args)
        {
            Options options = ProcessArgs(args);
            if (options.ShowHelp || args.Length == 0)
            {
                ShowHelp();
            }
            else if (options.ShowVersion)
            {
                ShowVersion();
            }
            else if (options.IsPromptMode)
            {
                EnterPromptMode(options);
            }
            else
            {
                EnterFileMode(options);
            }
        }

        /// <summary>
        /// Processes command line arguments.
        /// </summary>
        /// <param name="args">Arguments passed from the command line.</param>
        /// <returns>Options object.</returns>
        private static Options ProcessArgs(string[] args)
        {
            var options = new Options();
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-d":
                    case "--debug":
                        options.IsDebugMode = true;
                        break;
                    case "-p":
                    case "--prompt":
                        options.IsPromptMode = true;
                        break;
                    case "-h":
                    case "--help":
                        options.ShowHelp = true;
                        return options;
                    case "-v":
                    case "--version":
                        options.ShowVersion = true;
                        return options;
                    default:
                        options.FileName = arg;
                        break;
                }
            }

            return options;
        }

        /// <summary>
        /// Shows the help screen.
        /// </summary>
        private static void ShowHelp()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shows version information.
        /// </summary>
        private static void ShowVersion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enters into prompt mode.
        /// </summary>
        /// <param name="options">Options to use.</param>
        private static void EnterPromptMode(Options options)
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = options.IsDebugMode;
            while (true)
            {
                Console.Write(">>> ");
                string input = null;
                try
                {
                    input = Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An exception occured: {ex.Message}");
                }

                if (input == null)
                {
                    continue;
                }

                try
                {
                    var parser = new Parser.Parser(input);
                    BlockStatementNode block = parser.ParseBlock();
                    if (block != null)
                    {
                        var program = new ProgramNode(0, 1, 1);
                        program.Body = block;
                        evaluator.Run(program);

                        int count = block.Body.Count;
                        if (count > 0 && block.Body[count - 1].GetType() == typeof(ExitNode))
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ParseError || ex is RuntimeError)
                    {
                        Console.WriteLine($"{ex.GetType().Name}: {ex}");
                    }
                    else
                    {
                        Console.WriteLine($"ERROR: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Enters into file mode.
        /// </summary>
        /// <param name="options">Options to use.</param>
        private static void EnterFileMode(Options options)
        {
            throw new NotImplementedException();
        }
    }
}
