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
        /// Tests expressions which contain a single operation.
        /// </summary>
        [TestMethod]
        public void TestSingleOperations()
        {
            AssertEvaluation(15, "10 + 5");
            AssertEvaluation(5, "10 - 5");
            AssertEvaluation(100, "10 * 10");
            AssertEvaluation(50, "100 / 2");
        }

        /// <summary>
        /// Tests expressions which contain multiple operations.
        /// </summary>
        [TestMethod]
        public void TestComplexOperations()
        {
            AssertEvaluation(27, "(2 + 3) * 5 + 2");
        }

        /// <summary>
        /// Tests expressions in which multiple sets of parentheses are nested within each other.
        /// </summary>
        [TestMethod]
        public void TestNestedParentheses()
        {
            AssertEvaluation(145, "(10 + (5 + 2) * (10 + 10)) - 5");
            AssertEvaluation(15, "(((((10 + 5)))))");
            AssertEvaluation(60, "(((((10 + 5 * (((5 + 5))))))))");
        }

        /// <summary>
        /// Tests expressions containing variables.
        /// </summary>
        [TestMethod]
        public void TestVariables()
        {
            AssertEvaluation(10, "AA10 + AB5 + CD3");
        }
    }
}