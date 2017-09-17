using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTests
    {
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("_ 10 / 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("+ 5 * 10"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("!10 - 5"));
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(")10 - 5("));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + 5 _"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5 / 10 + "));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 - !5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5 * (10 + 5)+"));
        }

        /// <summary>
        /// Tests the syntax rule that any token that immediately follows an opening parenthesis 
        /// or an operator must be either a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod]
        public void PublicTestSyntaxParenthesisFollowing()
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(_ + 5)"));
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 _ 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 ... 5"));
            // Variable
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 _ 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A5 ... 5"));
            // Closing Parenthesis
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) _ 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) = 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) ... 5"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(5 + 5) (5 + 5)"));
        }
    }
}