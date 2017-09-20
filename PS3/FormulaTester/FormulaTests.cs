using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    /// <summary>
    /// Tests for the Formula project.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("       "));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("                      "));
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
        /// Tests variables which are not considered valid by the validator.
        /// </summary>
        [TestMethod]
        public void PublicTestNonValidatedVariable()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + AB", s => s, s => false));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + _cat", s => s, s => false));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + _cat - _dog", s => s, s => false));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + A123", s => s, s => false));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("10 + a123", s => s, s => false));
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
            Assert.ThrowsException<FormulaFormatException>(() =>
                new Formula("A_B&2 + 5")); // Includes invalid characters
        }

        /// <summary>
        /// Makes sure an exception is thrown when the expression passed in is null.
        /// </summary>
        [TestMethod]
        public void PublicTestExpressionIsNull()
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
            Assert.IsInstanceOfType(new Formula("0.12345 / (5.00 - (5.25 - 0.25))").Evaluate(s => s.Length),
                typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / (ABCD - 4)").Evaluate(s => s.Length), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / 0").Evaluate(s => s.Length), typeof(FormulaError));
        }

        /// <summary>
        /// Tests expressions with variable(s) that have no value.
        /// </summary>
        [TestMethod]
        public void PublicTestEvaluateVariableHasNoValue()
        {
            Assert.IsInstanceOfType(new Formula("5 / (AB - 10)").Evaluate(s => throw new ArgumentException()),
                typeof(FormulaError));
            Assert.IsInstanceOfType(
                new Formula("0.12345 / (5.00 - (_123 - 0.25))").Evaluate(s => throw new ArgumentException()),
                typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / (ABCD - 4)").Evaluate(s => throw new ArgumentException()),
                typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("1 / ab4").Evaluate(s => throw new ArgumentException()),
                typeof(FormulaError));
        }

        /// <summary>
        /// Tests the GetVariables method of Formula to make sure the correct variables are returned.
        /// </summary>
        [TestMethod]
        public void PublicTestGetVariables()
        {
            // Default Normalizer
            CollectionAssert.AreEqual(new[] {"ABC"}, new Formula("ABC").GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"abc", "ABC"}, new Formula("abc + ABC").GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"A10", "B6", "a4", "a10"},
                new Formula("A10 + 15 - (10 * B6) / a4 - a10").GetVariables().ToList());

            // Capital Normalizer
            CollectionAssert.AreEqual(new[] {"ABC"},
                new Formula("abc", s => s.ToUpper(), s => true).GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"ABC"},
                new Formula("abc - ABC", s => s.ToUpper(), s => true).GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"A", "B", "C", "D", "E"},
                new Formula("a - A + b - B * c / C * d - D + a - c + d + e / B", s => s.ToUpper(), s => true)
                    .GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"A", "D", "E", "F", "B", "C"},
                new Formula("a - d - e - f / b * c", s => s.ToUpper(), s => true).GetVariables().ToList());
            CollectionAssert.AreEqual(new[] {"A10", "B6", "A4"},
                new Formula("A10 + 15 - (10 * B6) / a4 - a10", s => s.ToUpper(), s => true).GetVariables().ToList());
        }

        /// <summary>
        /// Tests that ToString works correctly.
        /// </summary>
        [TestMethod]
        public void PublicTestToString()
        {
            // Default Normalizer
            Assert.AreEqual("10", new Formula("10").ToString());
            Assert.AreEqual("(10)", new Formula("(10)").ToString());
            Assert.AreEqual("10 + 5", new Formula("10+5").ToString());
            Assert.AreEqual("(10 - 5) + 5", new Formula("(10-5)+5").ToString());
            Assert.AreEqual("Ab - ac", new Formula("Ab- ac").ToString());
            Assert.AreEqual("(10 - 5) + 5 - AB * (_6c + c_d)", new Formula("(10-5)+5 - AB *(_6c +c_d)").ToString());
            Assert.AreEqual("(10 - 5) + 5 - AB * (_6c + c_d)",
                new Formula("(10.00000000-5.0000000000000)+5.0000000 - AB *(_6c +c_d)").ToString());

            // Capital Normalizer
            Assert.AreEqual("10", new Formula("10", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("(10)", new Formula("(10)", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("10 + 5", new Formula("10+5", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("(10 - 5) + 5", new Formula("(10-5)+5", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("AB - AC", new Formula("Ab- ac", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("(10 - 5) + 5 - AB * (_6C + C_D)",
                new Formula("(10-5)+5 - AB *(_6c +c_d)", s => s.ToUpper(), s => true).ToString());
            Assert.AreEqual("(10 - 5) + 5 - AB * (_6C + C_D)",
                new Formula("(10.00000000-5.0000000000000)+5.0000000 - AB *(_6c +c_d)", s => s.ToUpper(), s => true)
                    .ToString());
        }

        /// <summary>
        /// Tests that a formula created with another formula's ToString will equal that formula.
        /// </summary>
        [TestMethod]
        public void PublicTestToStringEquality()
        {
            // Default Normalizer
            var formula = new Formula("(10-5)+5 - AB *(_6c +c_d)");
            Assert.AreEqual(formula, new Formula(formula.ToString()));

            // Capital Normalizer
            formula = new Formula("(10-5)+5 - AB *(_6c +c_d)", s => s.ToUpper(), s => true);
            Assert.AreEqual(formula, new Formula(formula.ToString()));
        }

        /// <summary>
        /// Tests Equals, Hashcode, ==, and != for formulas.
        /// </summary>
        [TestMethod]
        public void PublicTestEquality()
        {
            // Idential expressions
            Assert.IsTrue(new Formula("10+5").Equals(new Formula("10+5")));
            Assert.AreEqual(new Formula("10+5").GetHashCode(), new Formula("10+5").GetHashCode());
            Assert.IsTrue(new Formula("10+5") == new Formula("10+5"));
            Assert.IsFalse(new Formula("10+5") != new Formula("10+5"));

            // Variable and spacing
            Assert.IsTrue(new Formula("10+AB").Equals(new Formula("10 + AB")));
            Assert.AreEqual(new Formula("10+AB").GetHashCode(), new Formula("10 + AB").GetHashCode());
            Assert.IsTrue(new Formula("10+AB") == new Formula("10 + AB"));
            Assert.IsFalse(new Formula("10+AB") != new Formula("10 + AB"));

            // Large spacing
            Assert.IsTrue(new Formula("10+5").Equals(new Formula("10    +     5")));
            Assert.AreEqual(new Formula("10+5").GetHashCode(), new Formula("10    +     5").GetHashCode());
            Assert.IsTrue(new Formula("10+5") == new Formula("10    +     5"));
            Assert.IsFalse(new Formula("10+5") != new Formula("10    +     5"));

            // Complex Equation
            Assert.IsTrue(
                new Formula("(10-5)+5 - AB *(_6c +c_d)").Equals(new Formula("(10 - 5) + 5 - AB * (_6c + c_d)")));
            Assert.AreEqual(new Formula("(10-5)+5 - AB *(_6c +c_d)").GetHashCode(),
                new Formula("(10 - 5) + 5 - AB * (_6c + c_d)").GetHashCode());
            Assert.IsTrue(new Formula("(10-5)+5 - AB *(_6c +c_d)") == new Formula("(10 - 5) + 5 - AB * (_6c + c_d)"));
            Assert.IsFalse(new Formula("(10-5)+5 - AB *(_6c +c_d)") != new Formula("(10 - 5) + 5 - AB * (_6c + c_d)"));
        }

        /// <summary>
        /// Tests passing null or non-formula objects into the equality methods.
        /// </summary>
        [TestMethod]
        public void PublicTestNullAndIncorrectEquals()
        {
            Assert.IsFalse(new Formula("10 + 5").Equals(null));
            Assert.IsFalse(new Formula("10 + 5") == null);
            Assert.IsFalse(null == new Formula("10 + 5"));

            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
        }

        /// <summary>
        /// Tests the FormulaError class' ability to store errors.
        /// </summary>
        [TestMethod]
        public void PublicTestFormulaError()
        {
            var error = new FormulaError("oh no");
            Assert.AreEqual("oh no", error.Reason);         
        }
    }
}