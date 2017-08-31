using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        /// <summary>
        /// This delegate is used to "look up" variables in the spreadsheet and get the values as integers.
        /// </summary>
        /// <param name="value">The variable name to look up.</param>
        /// <returns>The value of the variable.</returns>
        public delegate int Lookup(string value);

        /// <summary>
        /// This delegate is used to perform arithmetic for an operator. 
        /// For example, when used for a + operator, it would return left + right.
        /// </summary>
        /// <param name="left">The left input.</param>
        /// <param name="right">The right input.</param>
        /// <returns>
        /// The output of the left and right input computed by the operator that this action is assigned to: 
        /// left / right, 
        /// left * right, 
        /// etc.
        /// </returns>
        private delegate int OperatorAction(int left, int right);

        /// <summary>
        /// Contains valid Operators for performing arithmetic operations.
        /// </summary>
        private static readonly Dictionary<string, OperatorAction> OperatorDict = new Dictionary<string, OperatorAction>()
        {
            {"/", (left, right) => left / right},
            {"*", (left, right) => left * right},
            {"+", (left, right) => left + right},
            {"-", (left, right) => left - right}
        };

        /// <summary>
        /// Evaluates a string-based integer arithmetic expression and returns the integer result.
        /// For example, the expression "(2 + 3) * 5 + 2" will return 27.
        /// 
        /// The expression may also include variables, such as in "(2 + A6) * 5 + 2". Using the provided variableEvaluator,
        /// the variables will be looked up and turned into integers to perform the arithmetic evaluation.
        /// For example, the expression "(2 + A6) * 5 + 2" will return 47 when A6 is 7.
        /// </summary>
        /// <param name="expression">The expression to evaluate. Should not contain negative numbers.</param>
        /// <param name="variableEvaluator">The variable evaluation Lookup delegate.</param>
        /// <returns></returns>
        public static int Evaluate(string expression, Lookup variableEvaluator)
        {
            // Remove all whitespace from the expression.
            expression = Regex.Replace(expression, @"\s", "");

            // Split the expression up into individual tokens.
            var tokens = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // The value stack contains the actual integer values to perform operations on.
            var valueStack = new Stack<int>();

            // The operator stack contains the operators to apply to the values in the value stack: +, -, /, *, etc.
            var operatorStack = new Stack<string>();

            foreach (var token in tokens)
            {

                if (OperatorDict.ContainsKey(token))
                {
                    
                }

            }

            return 0;
        }
    }
}