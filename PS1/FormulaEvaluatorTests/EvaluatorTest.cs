using System;
using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEvaluatorTests
{
    /// <summary>
    /// Tests the Evaluator class against many cases.
    /// </summary>
    [TestClass]
    public class EvaluatorTest
    {
        /// <summary>
        /// Tests the evaluation of some simple expressions that do not contain any variables or edge cases.
        /// </summary>
        [TestMethod]
        public void TestEvaluationOfSimpleExpressions()
        {

            Assert.AreEqual(27, Evaluator.Evaluate(@"(2 + 3) * 5 + 2", null));

        }
    }
}
