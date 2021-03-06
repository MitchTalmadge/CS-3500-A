﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Normalizer = System.Func<string, string>;
using Validator = System.Func<string, bool>;

namespace SpreadsheetUtilities.Utils
{
    /// <summary>
    /// Provides helper / utility methods for parsing and evaluating expressions.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal static class ExpressionUtils
    {
        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        internal static IEnumerable<string> GetTokens(string expression)
        {
            // Pattern constants
            const string leftParenthesisPattern = @"\(";
            const string rightParenthesisPattern = @"\)";
            const string operatorPattern = @"[\+\-*/]";
            const string variablePattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            const string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            const string spacePattern = @"\s+";

            // Overall pattern
            var pattern =
                $"({leftParenthesisPattern}) | ({rightParenthesisPattern}) | ({operatorPattern}) | ({variablePattern}) | ({doublePattern}) | ({spacePattern})";

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (var token in Regex.Split(expression, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                // Ensure the token is not purely whitespace.
                if (!Regex.IsMatch(token, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return token;
                }
            }
        }

        /// <summary>
        /// Determines if the given token has the syntax of a variable.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token is a variable, false otherwise.</returns>
        internal static bool IsVariable(string token)
        {
            const string variablePattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
            return Regex.IsMatch(token, variablePattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Determines if the given token is a valid variable, both syntactically and literally.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <param name="normalizer">The variable normalizer.</param>
        /// <param name="validator">The variable validator.</param>
        /// <returns></returns>
        internal static bool IsValidVariable(string token, Normalizer normalizer, Validator validator)
        {
            return IsVariable(token) && validator(normalizer(token));
        }
    }
}