using System.Collections.Generic;
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
            // Patterns for individual tokens
            var lpPattern = @"\(";
            var rpPattern = @"\)";
            var opPattern = @"[\+\-*/]";
            var varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            var doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            var spacePattern = @"\s+";

            // Overall pattern
            var pattern =
                $"({lpPattern}) | ({rpPattern}) | ({opPattern}) | ({varPattern}) | ({doublePattern}) | ({spacePattern})";

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
        /// From the provided tokens, identifies and normalizes all variables, ignoring duplicates.
        /// Provides each variable in an enumerator in order of occurence.
        /// </summary>
        /// <param name="tokens">The tokens for the expression.</param>
        /// <param name="normalizer">The normalizer to use.</param>
        /// <returns>An enumerator that provides normalized variables in order of occurence without duplicates.</returns>
        internal static IEnumerable<string> GetNormalizedVariables(string[] tokens, Normalizer normalizer)
        {
            // The variables which have already been seen and returned, to prevent duplicate returns.
            var seenVariables = new HashSet<string>();

            // Iterate over each token in the expression.
            foreach (var token in tokens)
            {
                // Only handle tokens which are variables.
                if (!IsVariable(token))
                    continue;

                // Normalize the token.
                var normalizedToken = normalizer(token);

                // Check that we have already seen this token.
                if (seenVariables.Contains(normalizedToken))
                    continue;

                // Mark the token as seen.
                seenVariables.Add(normalizedToken);

                // Return the token.
                yield return normalizedToken;
            }
        }

        /// <summary>
        /// Determines if the given token has the syntax of a variable.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token is a variable, false otherwise.</returns>
        internal static bool IsVariable(string token)
        {
            const string pattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            return Regex.IsMatch(token, pattern);
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
