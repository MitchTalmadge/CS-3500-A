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
        private static int GetDummySpreadsheetValue(string variable)
        {
            return variable.Length;
        }

        /// <summary>
        /// Convenience method for evaluating expressions using the dummy lookup.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the evaluation.</returns>
        private static int Evaluate(string expression)
        {
            return Evaluator.Evaluate(expression, GetDummySpreadsheetValue);
        }

        /// <summary>
        /// Helper method for asserting whether the expected result is returned from an evaluation of an expression.
        /// </summary>
        /// <param name="expectedResult">The expected result after evaluation.</param>
        /// <param name="expression">The expression to evaluate.</param>
        private static void AssertEvaluation(int expectedResult, string expression)
        {
            Assert.AreEqual(expectedResult, Evaluate(expression));
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
            AssertEvaluation(1, "10 / (5 * 2)");
        }

        /// <summary>
        /// Tests expressions which may evaluate incorrectly if the order of operations are not correct.
        /// </summary>
        [TestMethod]
        public void TestOrderOfOperations()
        {
            // Multiplication
            AssertEvaluation(30, "5 + 5 * 5");
            AssertEvaluation(90, "10 * 10 - 10");

            // Division
            AssertEvaluation(6, "5 + 5 / 5");
            AssertEvaluation(-9, "10 / 10 - 10");

            // Parentheses
            AssertEvaluation(50, "(5 + 5) * 5");
            AssertEvaluation(2, "10 / (10 - 5)");
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
            AssertEvaluation(10, "AA10 + aB5 + Cd3");
            AssertEvaluation(14, "AB25 + 10");
            AssertEvaluation(60, "(a10 + b12) * 10");
            AssertEvaluation(30, "(7 + 3) * A14");
        }

        /// <summary>
        /// Tests expressions containing nothing more than a single value.
        /// </summary>
        [TestMethod]
        public void TestNoOperations()
        {
            AssertEvaluation(10, "10");
            AssertEvaluation(20, "(20)");
            AssertEvaluation(3, "A12");
            AssertEvaluation(4, "(AB12)");
        }

        /// <summary>
        /// Tests empty expressions, which should throw an exception.
        /// </summary>
        [TestMethod]
        public void TestEmptyExpressions()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate(""));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("()"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("        (           )          "));
        }

        /// <summary>
        /// Tests expressions that are missing parentheses, which should throw exceptions.
        /// </summary>
        [TestMethod]
        public void TestMissingParentheses()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("(5+5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5+5)"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 * (5 + (10 * 4)"));
        }

        /// <summary>
        /// Tests expressions that are missing operators.
        /// </summary>
        [TestMethod]
        public void TestMissingOperators()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5 5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 * 5 + 7 (10 + 5)"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("0 - 1 5"));
        }

        /// <summary>
        /// Tests expressions that are missing values.
        /// </summary>
        [TestMethod]
        public void TestMissingValues()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5 - "));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("* 10 + 15"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("11 - 6 + (10 -) + 1"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("+"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("+-*/()"));
        }

        /// <summary>
        /// Tests expressions whose operators are not in the correct place.
        /// </summary>
        [TestMethod]
        public void TestMisplacedOperators()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("(10 +) 5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5 (+ 10)"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5 ((+ 10))"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5 ((+ ((10 + 5)) + 10))"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("+ 5 10"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 5 +"));
        }

        /// <summary>
        /// Tests expressions that contain negative numbers, which are not supported.
        /// </summary>
        [TestMethod]
        public void TestNegativeNumbers()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 + -5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("(-5) + 10"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("-10 * -20"));
        }

        /// <summary>
        /// Tests expressions that contain unknown/invalid characters or operators.
        /// </summary>
        [TestMethod]
        public void TestInvalidCharacters()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 % 5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("[10 + 2] - 5"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5^10"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("5_10_20"));
        }

        /// <summary>
        /// Tests expressions that invalidly formatted variables.
        /// </summary>
        [TestMethod]
        public void TestInvalidVariableFormats()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("AAB10B - 100"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10AB + 10"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("6 + (AB_10 - 10)"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10 - 5 * (test + 5)"));
        }

        /// <summary>
        /// Tests expressions which involve dividing by zero.
        /// </summary>
        [TestMethod]
        public void TestDivideByZero()
        {
            Assert.ThrowsException<ArgumentException>(() => Evaluate("10/0"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("0/0"));
            Assert.ThrowsException<ArgumentException>(() => Evaluate("120 * 5 / (15 + 10 - 25)"));
        }
    }
}