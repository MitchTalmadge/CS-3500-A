using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpreadsheetUtilities.Utils
{
    /// <summary>
    /// Provides useful helper / utility methods for parsing and evaluating formulas.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal static class FormulaUtils
    {
        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        internal static IEnumerable<string> GetTokens(string formula)
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
            foreach (var token in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                // Ensure the token is not purely whitespace.
                if (!Regex.IsMatch(token, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return token;
                }
            }
        }

        /// <summary>
        /// Checks if a given token is a double, and returns the parsed double if it is.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>The parsed double if it is a double, or null if it is not.</returns>
        internal static double? ToDouble(string token)
        {
            // Try to parse the token as a double.
            if (double.TryParse(token, out var parsed))
                return parsed;

            // Unable to parse.
            return null;
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

    }
}
