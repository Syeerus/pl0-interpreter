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
    }
}
