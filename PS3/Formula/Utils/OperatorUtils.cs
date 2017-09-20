using System.Collections.Generic;
using SpreadsheetUtilities.Operators;
using SpreadsheetUtilities.Operators.Arithmetic;

namespace SpreadsheetUtilities.Utils
{
    /// <summary>
    /// Provides helper / utility methods for working with Operator instances.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    public class OperatorUtils
    {
        /// <summary>
        /// Contains a mapping of tokens to Operator instances for use within expression evaluations.
        /// Useful for logic which depends on a token being an operator.
        /// </summary>
        internal static readonly Dictionary<string, Operator> OperatorDict = new Dictionary<string, Operator>
        {
            {"(", new GroupingOperator()},
            {")", new GroupingOperator(true)},
            {"+", new AdditionArithmeticOperator()},
            {"-", new SubtractionArithmeticOperator()},
            {"*", new MultiplicationArithmeticOperator()},
            {"/", new DivisionArithmeticOperator()}
        };

        /// <summary>
        /// Checks if a token is an opening Grouping Operator.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token opens a group.</returns>
        internal static bool IsOpeningGroupOperator(string token)
        {
            if (!OperatorDict.TryGetValue(token, out var op))
                return false;

            return op is GroupingOperator groupOp && groupOp.OpensGroup;
        }

        /// <summary>
        /// Checks if a token is a closing Grouping Operator.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token closes a group.</returns>
        internal static bool IsClosingGroupOperator(string token)
        {
            if (!OperatorDict.TryGetValue(token, out var op))
                return false;

            return op is GroupingOperator groupOp && !groupOp.OpensGroup;
        }

        /// <summary>
        /// Checks if a token is an Arithmetic Operator.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True if the token is an Arithmetic Operator.</returns>
        internal static bool IsArithmeticOperator(string token)
        {
            if (!OperatorDict.TryGetValue(token, out var op))
                return false;

            return op is ArithmeticOperator;
        }
    }
}