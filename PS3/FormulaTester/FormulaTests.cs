﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// Tests the syntax rule that floating point numbers are acceptable.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxFloatingPointNumbers()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("10.5 + 5"));
            Assert.IsNotNull(new Formula("11.123456 - 1.23445 * (150.3984 / 1082.43434)"));
        }

        /// <summary>
        /// Tests the syntax rule that there must be at least one token.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxAtLeastOneToken()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(""));
        }

        /// <summary>
        /// Tests the syntax rule that when reading tokens from left to right, at no point should the number of 
        /// closing parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxRightParentheses()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("(10 + 5)"));
            Assert.IsNotNull(new Formula("((10 + 5))"));

            // Should throw
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(10 + 5))"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("((10 + 5)))"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("((10 + 5) + (5 + (5 * 10)) 10))"));
        }

        /// <summary>
        /// Tests the syntax rule that the total number of opening parentheses must equal the total number of closing parentheses.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxBalancedParentheses()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("(10 + 5)"));
            Assert.IsNotNull(new Formula("((10 - 5))"));

            // Should Throw
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(10 + 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("((10 / 5)"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(((10 - 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(((10 + 5) * (5 + (5 * 10)) 10)"));
        }

        /// <summary>
        /// Tests the syntax rule that the first token of an expression must be a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxStartingToken()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("(10 + 5)")); // Start with opening parenthesis
            Assert.IsNotNull(new Formula("10 - 5")); // Start with number
            Assert.IsNotNull(new Formula("A5 * 5")); // Start with variable

            // Should Throw
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(")10 + 5("));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("/ 10 / 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("+ 5 * 10"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("$10 - 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("+(10 + 5) * 5"));
        }

        /// <summary>
        /// Tests the syntax rule that the last token of an expression must be a number, a variable, or a closing parenthesis.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxLastToken()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("(10 + 5)")); // End with closing parenthesis.
            Assert.IsNotNull(new Formula("10 - 5")); // End with number.
            Assert.IsNotNull(new Formula("5 * A5")); // End with variable.

            // Should Throw
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(10 - 5("));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + 5 %"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5 / 10 + "));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 - 5$"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5 * (10 + 5)+"));
        }

        /// <summary>
        /// Tests the syntax rule that any token that immediately follows an opening parenthesis 
        /// or an operator must be either a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxParenthesisOperatorFollowing()
        {
            // Shouldn't throw
            // Parenthesis
            Assert.IsNotNull(new Formula("(10 + 5)")); // Number.
            Assert.IsNotNull(new Formula("(A5 / 5)")); // Variable.
            Assert.IsNotNull(new Formula("((10 * 5))")); // Opening Parenthesis.
            Assert.IsNotNull(new Formula("((((((((((10 - 5))))))))))")); // Opening Parenthesis.
            // Operator
            Assert.IsNotNull(new Formula("10 + 5")); // Number.
            Assert.IsNotNull(new Formula("10 * A5")); // Variable.
            Assert.IsNotNull(new Formula("10 - (10 + 5)")); // Opening Parenthesis.

            // Should Throw
            // Parenthesis
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("()"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(; + 5)"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(+ 5)"));

            // Operator
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + /"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 - *5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(10 * )"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 / )"));
        }

        /// <summary>
        /// Tests the syntax rule that any token that immediately follows a number, a variable, 
        /// or a closing parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxExtraFollowing()
        {
            // Shouldn't throw
            // Number
            Assert.IsNotNull(new Formula("(10 + 5)")); // Closing Parenthesis.
            Assert.IsNotNull(new Formula("10 + 5")); // Operator.
            Assert.IsNotNull(new Formula("10 - 5")); // Operator.
            Assert.IsNotNull(new Formula("10 * 5")); // Operator.
            Assert.IsNotNull(new Formula("10 / 5")); // Operator.
            // Variable
            Assert.IsNotNull(new Formula("(10 + A5)")); // Closing Parenthesis.
            Assert.IsNotNull(new Formula("A5 + 10")); // Operator.
            Assert.IsNotNull(new Formula("A5 - 10")); // Operator.
            Assert.IsNotNull(new Formula("A5 * 10")); // Operator.
            Assert.IsNotNull(new Formula("A5 / 10")); // Operator.
            // Closing Parenthesis
            Assert.IsNotNull(new Formula("((10 + 5))")); // Closing Parenthesis.
            Assert.IsNotNull(new Formula("(5 + 5) + 10")); // Operator.
            Assert.IsNotNull(new Formula("(5 / 5) - 10")); // Operator.
            Assert.IsNotNull(new Formula("(5 / 5) * 10")); // Operator.
            Assert.IsNotNull(new Formula("(5 / 5) / 10")); // Operator.

            // Should Throw
            // Number
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 AB 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 ... 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 5"));
            // Variable
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 AB 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 ... 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 5"));
            // Closing Parenthesis
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) ^ 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) ... 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) (5 + 5)"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) 5"));
        }

        /// <summary>
        /// Tests the default validation delegate which returns true for variables consisting of any letter or 
        /// underscore followed by any number of letters, digits, and/or underscores.
        /// </summary>
        [TestMethod]
        public void PublicTestDefaultValidatorDelegate()
        {
            // Shouldn't throw
            Assert.IsNotNull(new Formula("A + 1")); // One letter
            Assert.IsNotNull(new Formula("a + 1")); // One letter, lowercase
            Assert.IsNotNull(new Formula("A1 + 1")); // One letter and one number
            Assert.IsNotNull(new Formula("a1 + 1")); // One letter and one number, lowercase
            Assert.IsNotNull(new Formula("A1B + 1")); // Letter followed by number and letter.
            Assert.IsNotNull(new Formula("A1_ + 1")); // Letter followed by number and underscore.
            Assert.IsNotNull(new Formula("A_ + 1")); // Letter followed by underscore.
            Assert.IsNotNull(new Formula("_ + 1")); // Underscore only.
            Assert.IsNotNull(new Formula("____ + 1")); // Multiple Underscores.
            Assert.IsNotNull(new Formula("_B_D10_A123_ + 1")); // Mixed.
            Assert.IsNotNull(new Formula("A1234_5678B + 1")); // Mixed.
            Assert.IsNotNull(new Formula("A_1_B______C2 + 1")); // Mixed.

            // Should throw
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1A + 5")); // Starts with number
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("AB% + 5")); // Includes invalid characters
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("AB$ + 5")); // Includes invalid characters
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A$B + 5")); // Includes invalid characters
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A_B&2 + 5")); // Includes invalid characters
        }

        /// <summary>
        /// Makes sure an exception is thrown when the formula passed in is null.
        /// </summary>
        [TestMethod]
        public void PublicTestFormulaIsNull()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(null));
        }

        /// <summary>
        /// Tests expressions which contain a single operation.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateSingleOperations()
        {
            Assert.AreEqual(15.5d, new Formula("10.5 + 5").Evaluate(s => s.Length));
            Assert.AreEqual(5.5d, new Formula("10.75 - 5.25").Evaluate(s => s.Length));
            Assert.AreEqual(100d, new Formula("10 * 10").Evaluate(s => s.Length));
            Assert.AreEqual(50d, new Formula("100 / 2").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions which contain multiple operations.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateComplexOperations()
        {
            Assert.AreEqual(27d, new Formula("(2 + 3) * 5 + 2").Evaluate(s => s.Length));
            Assert.AreEqual(1d, new Formula("10 / (5 * 2)").Evaluate(s => s.Length));
            Assert.AreEqual(23d, new Formula("((1 + (6 + 4)) + (4 + 8))").Evaluate(s => s.Length));
            Assert.AreEqual(0.25d, new Formula("10.5 - (50.80 - 40.55)").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions which may evaluate incorrectly if the order of operations are not correct.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateOrderOfOperations()
        {
            // Multiplication
            Assert.AreEqual(30d, new Formula("5 + 5 * 5").Evaluate(s => s.Length));
            Assert.AreEqual(90d, new Formula("10 * 10 - 10").Evaluate(s => s.Length));

            // Division
            Assert.AreEqual(6d, new Formula("5 + 5 / 5").Evaluate(s => s.Length));
            Assert.AreEqual(-9d, new Formula("10 / 10 - 10").Evaluate(s => s.Length));

            // Parentheses
            Assert.AreEqual(50d, new Formula("(5 + 5) * 5").Evaluate(s => s.Length));
            Assert.AreEqual(2d, new Formula("10 / (10 - 5)").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions in which multiple sets of parentheses are nested within each other.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateNestedParentheses()
        {
            Assert.AreEqual(145d, new Formula("(10 + (5 + 2) * (10 + 10)) - 5").Evaluate(s => s.Length));
            Assert.AreEqual(15d, new Formula("(((((10 + 5)))))").Evaluate(s => s.Length));
            Assert.AreEqual(60d, new Formula("(((((10 + 5 * (((5 + 5))))))))").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions containing variables.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateVariables()
        {
            Assert.AreEqual(10d, new Formula("AA10 + aB5 + Cd3").Evaluate(s => s.Length));
            Assert.AreEqual(14d, new Formula("AB25 + 10").Evaluate(s => s.Length));
            Assert.AreEqual(60d, new Formula("(a10 + b12) * 10").Evaluate(s => s.Length));
            Assert.AreEqual(30d, new Formula("(7 + 3) * A14").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions containing nothing more than a single value.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateNoOperations()
        {
            Assert.AreEqual(10d, new Formula("10").Evaluate(s => s.Length));
            Assert.AreEqual(20d, new Formula("(20)").Evaluate(s => s.Length));
            Assert.AreEqual(3d, new Formula("A12").Evaluate(s => s.Length));
            Assert.AreEqual(4d, new Formula("(AB12)").Evaluate(s => s.Length));
        }

        /// <summary>
        /// Tests expressions with divide-by-zero errors.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateDivideByZero()
        {
            Assert.IsInstanceOfType(new Formula("5 / (10 - 10)").Evaluate(s => s.Length), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("0.12345 / (5.00 - (5.25 - 0.25))").Evaluate(s => s.Length), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / (ABCD - 4)").Evaluate(s => s.Length), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / 0").Evaluate(s => s.Length), typeof(FormulaError));
        }
    }

}