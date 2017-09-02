using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FormulaEvaluator.Operators;
using FormulaEvaluator.Operators.Arithmetic;

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
        /// Contains a mapping of tokens to Operator instances for use within expression evaluations.
        /// Useful for adding Operators to the operatorStack when parsing tokens.
        /// </summary>
        private static readonly Dictionary<string, Operator> OperatorDict = new Dictionary<string, Operator>
        {
            {"(", new GroupingOperator()},
            {")", new GroupingOperator(true)},
            {"+", new AdditionArithmeticOperator()},
            {"-", new SubtractionArithmeticOperator()},
            {"*", new MultiplicationArithmeticOperator()},
            {"/", new DivisionArithmeticOperator()}
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

            // The operator stack contains the Operators that will be applied to the values in the value stack: +, -, /, *, etc.
            var operatorStack = new Stack<Operator>();

            // Iterate over each token to determine its type and how it should be handled.
            foreach (var token in tokens)
            {
                // Check if this token is an operator.
                if (OperatorDict.ContainsKey(token))
                {
                    var currentOperator = OperatorDict[token];
                    // Determine the type of Operator.
                    if (currentOperator is GroupingOperator groupingOperator)
                    {
                        // This is a Grouping Operator, i.e. parentheses. 

                        // Check if the grouping operator opens or closes the group.
                        if (groupingOperator.OpensGroup)
                        {
                            // Opens the group. Simply add to the stack.
                            operatorStack.Push(groupingOperator);
                        }
                        else
                        {
                            // Closes the group. 
                            CloseGroup(valueStack, operatorStack);
                        }
                    }
                    else if (currentOperator is ArithmeticOperator arithmeticOperator)
                    {
                        if (arithmeticOperator.HighLevel)
                        {
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    // This is either a number or a variable.
                }
            }

            return 0;
        }

        /// <summary>
        /// Performs the required operations on the value and operator stacks when an expression group is closed.
        /// </summary>
        /// <param name="valueStack">The stack containing integer values.</param>
        /// <param name="operatorStack">The stack containing the Operators to perform on the values.</param>
        private static void CloseGroup(Stack<int> valueStack, Stack<Operator> operatorStack)
        {
            // Check that there is an Arithmetic Operator to compute within the group.
            if (operatorStack.Peek() is ArithmeticOperator)
            { 
                // Perform computation.
                ComputeWithTopOperator(valueStack, operatorStack);
            }

            // Check for a closing Group Operator to remove.
            if (!(operatorStack.Pop() is GroupingOperator groupingOperator && groupingOperator.OpensGroup))
            {
                throw new InvalidOperationException(
                    "Could not find an opening group operator to close. Is the expression missing a parenthesis?");
            }

            // Check if there is a high level Arithmetic Operator that needs to be computed.
            if (operatorStack.Peek() is ArithmeticOperator arithmeticOperator && arithmeticOperator.HighLevel)
            {
                // Perform computation.
                ComputeWithTopOperator(valueStack, operatorStack);
            }

        }

        /// <summary>
        /// Computes the value when the Operator at the top of the operatorStack is applied to the two top-most values in the valueStack.
        /// Stores the computed value at the top of the valueStack.
        /// 
        /// The Operator at the top of the operatorStack should be an Arithmetic Operator.
        /// </summary>
        /// <param name="valueStack">The stack containing at least two values.</param>
        /// <param name="operatorStack">The stack containing an Arithmetic Operator at the top.</param>
        private static void ComputeWithTopOperator(Stack<int> valueStack, Stack<Operator> operatorStack)
        {
            if (operatorStack.Pop() is ArithmeticOperator arithmeticOperator)
            {
                // Pop two values and compute them against the operator. Store the result in the value stack.
                var values = PopTwoValues(valueStack);
                valueStack.Push(arithmeticOperator.Compute(values.Item1, values.Item2));
                return;
            }

            throw new InvalidOperationException(
                "The Operator at the top of the operatorStack was not an Arithmetic Operator.");
        }

        /// <summary>
        /// Pops two values from the top of the value stack and returns them in a Tuple.
        /// The first value popped is item1, and the second is item2.
        /// </summary>
        /// <param name="valueStack">The stack containing at least two values.</param>
        /// <returns>A Tuple containing the two values popped.</returns>
        private static Tuple<int, int> PopTwoValues(Stack<int> valueStack)
        {
            try
            {
                return new Tuple<int, int>(valueStack.Pop(), valueStack.Pop());
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("There are not enough values to pop.", e);
            }
        }
    }
}