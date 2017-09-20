using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities.Utils;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Used for checking the syntax of expressions.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal static class ExpressionSyntaxChecker
    {

        /// <summary>
        /// Ensures that the provided expression can be evaluated without error.
        /// Throws a FormulaFormatException if there are any problems.
        /// </summary>
        /// <param name="expression">The expression to check.</param>
        /// <param name="normalizer">The variable normalizer.</param>
        /// <param name="validator">The variable validator.</param>
        internal static void CheckSyntax(string expression, Func<string, string> normalizer, Func<string, bool> validator)
        {
            // Check for null or empty formulas.
            if (expression == null)
                throw new FormulaFormatException("The expression is null and cannot be parsed.");
            if (expression.Trim() == "")
                throw new FormulaFormatException("The expression is empty and cannot be parsed.");

            // Get an enumerator for all the tokens in the formula.
            var tokens = ExpressionUtils.GetTokens(expression).GetEnumerator();

            // Get the first token out and ensure it is legal.
            // The first token must be either an opening parenthesis, a number, or a variable.
            tokens.MoveNext();
            var token = tokens.Current;
            if (!OperatorUtils.IsOpeningGroupOperator(token)
                && !double.TryParse(token, out _)
                && !(ExpressionUtils.IsVariable(token) && validator(normalizer(token))))
                throw new FormulaFormatException(
                    "The first token of the expression must be an opening parenthesis, a number, or a variable.");

            // Keep track of number of opening and closing parentheses.
            int numOpeningParentheses = 0, numClosingParentheses = 0;

            // Keep track of the last token seen
            string lastToken = null;

            // Iterate over each token in the enumerator.
            do
            {
                token = tokens.Current;

                if (lastToken != null)
                {
                    /* Check that any token immediately following an opening parenthesis or operator 
                     * is either an opening parenthesis, a number, or a variable. */
                    if (OperatorUtils.IsOpeningGroupOperator(lastToken)
                        || OperatorUtils.IsArithmeticOperator(lastToken))
                    {
                        if (!OperatorUtils.IsOpeningGroupOperator(token)
                            && !double.TryParse(token, out _)
                            && !(ExpressionUtils.IsVariable(token) && validator(normalizer(token))))
                            throw new FormulaFormatException(
                                "Any tokens following an opening parenthesis or operator must be an opening parenthesis, a number, or a variable.\n" +
                                $"The token '{token}' is incorrectly following the token '{lastToken}'");
                    }

                    /* Check that any token immediately following a number, a variable, or a closing parenthesis 
                     * is either an operator or a closing parenthesis */
                    if (OperatorUtils.IsClosingGroupOperator(lastToken)
                        || double.TryParse(lastToken, out _)
                        || ExpressionUtils.IsVariable(lastToken) && validator(normalizer(lastToken)))
                    {
                        if (!OperatorUtils.IsClosingGroupOperator(token)
                            && !OperatorUtils.IsArithmeticOperator(token))
                            throw new FormulaFormatException(
                                "Any tokens following a closing parenthesis, number, or variable must be a closing parenthesis or operator.\n" +
                                $"The token '{token}' is incorrectly following the token '{lastToken}'");
                    }
                }

                // Check parentheses.
                if (OperatorUtils.IsOpeningGroupOperator(token))
                {
                    // Increase the counter for opening parentheses.
                    numOpeningParentheses++;
                }
                else if (OperatorUtils.IsClosingGroupOperator(token))
                {
                    // Increase the counter for closing parentheses.
                    numClosingParentheses++;

                    // Ensure we have not seen more closing parentheses than opening.
                    if (numClosingParentheses > numOpeningParentheses)
                        throw new FormulaFormatException(
                            "There are too many closing parentheses in the expression.");
                }

                lastToken = token;
            } while (tokens.MoveNext());
            // Dispose of tokens enumerator.
            tokens.Dispose();

            // Check that the last token is legal.
            // The last token must be a closing parenthesis, a number, or a variable.
            if (!OperatorUtils.IsClosingGroupOperator(token)
                && !double.TryParse(token, out _)
                && !(ExpressionUtils.IsVariable(token) && validator(normalizer(token))))
                throw new FormulaFormatException(
                    "The last token of the expression must be a closing parenthesis, a number, or a variable.");

            // Make sure the parentheses are balanced (opening == closed)
            if (numOpeningParentheses != numClosingParentheses)
                throw new FormulaFormatException(
                    "Parentheses are unbalanced; the number of opening parentheses does not match the number of closing parentheses.");
        }

    }
}
