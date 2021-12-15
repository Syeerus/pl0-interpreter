/**
 * Evaluator unit tests.
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
using Interpreter;
using Interpreter.Errors;

namespace Tests
{
    [TestClass]
    public class EvaluatorTests
    {
        [TestMethod]
        public void SmallestProgramTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run(".");
        }

        [TestMethod]
        public void PrintConstantTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("const a = 5; ! a.");
        }

        [TestMethod]
        public void PrintVariableTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a; begin a := 1 + 2; ! a end.");
        }

        [TestMethod]
        public void ProcedureTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("procedure print; ! 'Hello world'; call print.");
        }

        [TestMethod]
        public void ReassignConstantTest()
        {
            var evaluator = new Evaluator();
            var source = "const a = 1; a := 2 .";
            Assert.ThrowsException<ReassignConstantError>(() => evaluator.Run(source));
        }

        [TestMethod]
        public void ReassignVariableTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a; begin a := 1; a := 'Hello world'; ! a end.");
        }

        [TestMethod]
        public void UnaryExpressionTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("! -(1 + 2).");
        }

        [TestMethod]
        public void RedeclareTest()
        {
            var evaluator = new Evaluator();
            var source = "procedure test; ; procedure test; ;.";
            Assert.ThrowsException<RedeclareError>(() => evaluator.Run(source));
        }

        [TestMethod]
        public void TypecastTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a, b; begin a := (int)'5'; b := a + 1; ! a end.");
        }

        [TestMethod]
        public void DivideByZeroTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            var source = "var a; a := 1 / (1 - 1).";
            Assert.ThrowsException<DivideByZeroError>(() => evaluator.Run(source));
        }

        [TestMethod]
        public void MixingTypesTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a; begin a := 1 + 2.5; a := 1.5 + 2; ! a end.");
        }

        [TestMethod]
        public void CallUndeclaredProcedureTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            var source = "call test.";
            Assert.ThrowsException<NameError>(() => evaluator.Run(source));
        }

        [TestMethod]
        public void PrintNullVariableTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a; ! a.");
        }

        [TestMethod]
        public void NullVariableOperationTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            var source = "var a; ! a + 1 .";
            Assert.ThrowsException<TypeError>(() => evaluator.Run(source));
        }

        [TestMethod]
        public void IfTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("if 1 = 1 then ! '1 is equal to 1'.");
            evaluator.Run("if 'hello' = 'hello' then ! 'hello is equal to hello'.");
            evaluator.Run("if odd 1 + 2 then ! '1 + 2 is odd'.");
        }

        [TestMethod]
        public void WhileTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("var a; begin a := 0; while a < 5 do begin ! a; a := a + 1 end end.");
        }

        [TestMethod]
        public void NestedProcedureTest()
        {
            var evaluator = new Evaluator();
            evaluator.IsDebugMode = true;
            evaluator.Run("procedure one; procedure two; ! 'Inside of two'; call two call one.");
        }
    }
}
