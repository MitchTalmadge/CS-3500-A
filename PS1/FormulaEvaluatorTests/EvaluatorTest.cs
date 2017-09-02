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
        /// A dummy spreadsheet variable evaluator that uses the length of the
        /// provided variable as the returned value.
        /// </summary>
        /// <param name="variable">The variable to evaluate.</param>
        /// <returns>The length of the variable.</returns>
        private int GetDummySpreadsheetValue(string variable)
        {
            return variable.Length;
        }

        /// <summary>
        /// Helper method for asserting whether the expected result is returned from an evaluation of an expression.
        /// </summary>
        /// <param name="expectedResult">The expected result after evaluation.</param>
        /// <param name="expression">The expression to evaluate.</param>
        private void AssertEvaluation(int expectedResult, string expression)
        {
            Assert.AreEqual(expectedResult, Evaluator.Evaluate(expression, GetDummySpreadsheetValue));
        }

        /// <summary>
        /// Tests the evaluation of some simple expressions that do not contain any variables or edge cases.
        /// </summary>
        [TestMethod]
        public void TestEvaluationOfSimpleExpressions()
        {
            AssertEvaluation(27, "(2 + 3) * 5 + 2");
        }
    }
}