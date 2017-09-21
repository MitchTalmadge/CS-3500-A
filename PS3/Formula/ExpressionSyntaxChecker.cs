﻿using SpreadsheetUtilities.Utils;
using Normalizer = System.Func<string, string>;
using Validator = System.Func<string, bool>;

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
        internal static void CheckSyntax(string expression, Normalizer normalizer, Validator validator)
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
            CheckFirstToken(token, normalizer, validator);

            // Keep track of number of opening and closing parentheses.
            int numOpeningParentheses = 0, numClosingParentheses = 0;

            // Keep track of the previous token seen
            string previousToken = null;

            // Iterate over each token in the enumerator.
            do
            {
                token = tokens.Current;

                // Check tokens which follow opening parentheses or operators.
                CheckFollowingOpeningParenthesesOperators(previousToken, token, normalizer, validator);

                // Check tokens which follow closing parentheses, numbers, or variables.
                CheckFollowingClosingParenthesesNumbersVariables(previousToken, token, normalizer, validator);

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

                previousToken = token;
            } while (tokens.MoveNext());
            // Dispose of tokens enumerator.
            tokens.Dispose();

            // Check the final token.
            CheckFinalToken(token, normalizer, validator);

            // Make sure the parentheses are balanced (opening == closed)
            if (numOpeningParentheses != numClosingParentheses)
                throw new FormulaFormatException(
                    "Parentheses are unbalanced; the number of opening parentheses does not match the number of closing parentheses.");
        }

        /// <summary>
        /// Ensures that the final token of an expression is an opening parenthesis, a number, or a variable.
        /// Throws if not legal.
        /// </summary>
        /// <param name="token">The final token of the expression.</param>
        /// <param name="normalizer">The normalizer.</param>
        /// <param name="validator">The validator.</param>
        private static void CheckFirstToken(string token, Normalizer normalizer, Validator validator)
        {
            if (!OperatorUtils.IsOpeningGroupOperator(token)
                && !double.TryParse(token, out _)
                && !(ExpressionUtils.IsVariable(token) && validator(normalizer(token))))
                throw new FormulaFormatException(
                    "The first token of the expression must be an opening parenthesis, a number, or a variable.");
        }

        /// <summary>
        /// Ensures that the final token of an expression is a closing parenthesis, a number, or a variable.
        /// Throws if not legal.
        /// </summary>
        /// <param name="token">The final token of the expression.</param>
        /// <param name="normalizer">The normalizer.</param>
        /// <param name="validator">The validator.</param>
        private static void CheckFinalToken(string token, Normalizer normalizer, Validator validator)
        {
            if (!OperatorUtils.IsClosingGroupOperator(token)
                && !double.TryParse(token, out _)
                && !(ExpressionUtils.IsVariable(token) && validator(normalizer(token))))
                throw new FormulaFormatException(
                    "The last token of the expression must be a closing parenthesis, a number, or a variable.");
        }

        /// <summary>
        /// Ensures that any token immediately following an opening parenthesis or operator
        /// is either another opening parenthesis, a number, or a variable.
        /// </summary>
        /// <param name="previousToken">The previous token iterated over. Null if this is the first.</param>
        /// <param name="currentToken">The current token being checked.</param>
        /// <param name="normalizer">The normalizer.</param>
        /// <param name="validator">The validator.</param>
        private static void CheckFollowingOpeningParenthesesOperators(
            string previousToken,
            string currentToken,
            Normalizer normalizer,
            Validator validator)
        {
            // Nothing to check if previous is null.
            if (previousToken == null)
                return;

            // Make sure the previous token was either an opening group operator or an arithmetic operator.
            if (!OperatorUtils.IsOpeningGroupOperator(previousToken)
                && !OperatorUtils.IsArithmeticOperator(previousToken))
                return;

            // Make sure the current token is either an opening group operator, a number, or a variable.
            if (!OperatorUtils.IsOpeningGroupOperator(currentToken)
                && !double.TryParse(currentToken, out _)
                && !(ExpressionUtils.IsVariable(currentToken) && validator(normalizer(currentToken))))
                throw new FormulaFormatException(
                    "Any tokens following an opening parenthesis or operator must be an opening parenthesis, a number, or a variable.\n" +
                    $"The token '{currentToken}' is incorrectly following the token '{previousToken}'");
        }

        /// <summary>
        /// Ensures that any token immediately following a closing parenthesis, number, or variable
        /// is either another closing parenthesis or an operator.
        /// </summary>
        /// <param name="previousToken">The previous token iterated over. Null if this is the first.</param>
        /// <param name="currentToken">The current token being checked.</param>
        /// <param name="normalizer">The normalizer.</param>
        /// <param name="validator">The validator.</param>
        private static void CheckFollowingClosingParenthesesNumbersVariables(
            string previousToken,
            string currentToken,
            Normalizer normalizer,
            Validator validator)
        {
            // Nothing to check if previous is null.
            if (previousToken == null)
                return;

            // Make sure the previous token was either a closing group operator, a number, or a variable.
            if (!OperatorUtils.IsClosingGroupOperator(previousToken)
                && !double.TryParse(previousToken, out _)
                && (!ExpressionUtils.IsVariable(previousToken) || !validator(normalizer(previousToken))))
                return;

            // Make sure the current token is either a closing group operator or an arithmetic operator.
            if (!OperatorUtils.IsClosingGroupOperator(currentToken)
                && !OperatorUtils.IsArithmeticOperator(currentToken))
                throw new FormulaFormatException(
                    "Any tokens following a closing parenthesis, number, or variable must be a closing parenthesis or operator.\n" +
                    $"The token '{currentToken}' is incorrectly following the token '{previousToken}'");
        }
    }
}