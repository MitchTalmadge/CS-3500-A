using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FormulaEvaluator.Operators;
using FormulaEvaluator.Operators.Arithmetic;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class provides a static method, Evaluate, which is used to evaluate infix expressions.
    /// The expressions may also contain variables, such as "AB2".
    /// 
    /// The main purpose of this class is to provide the functionality for evaluating expressions in a spreadsheet application.
    /// </summary>
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
        /// A Regex pattern for matching variables in an expression.
        /// The pattern will match strings that consist of one or more letters followed by one or more numbers:
        /// AA10
        /// AB15
        /// AAA2450
        /// ...
        /// </summary>
        private static readonly Regex ExpressionVariableRegex = new Regex(@"^[a-zA-Z]+\d+$");

        /// <summary>
        /// A Regex pattern for splitting expressions into individual tokens.
        /// </summary>
        private static readonly Regex ExpressionTokenSplitRegex = new Regex(@"(\()|(\))|(-)|(\+)|(\*)|(/)|(\s+)");

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
            // Split the expression up into individual tokens.
            var tokens = ExpressionTokenSplitRegex.Split(expression);

            // The value stack contains the actual integer values to perform operations on.
            var valueStack = new Stack<int>();

            // The operator stack contains the Operators that will be applied to the values in the value stack: +, -, /, *, etc.
            var operatorStack = new Stack<Operator>();

            // Iterate over each token to determine its type and how it should be handled.
            foreach (var token in tokens)
            {
                // Skip empty tokens.
                if (token.Trim().Length == 0)
                    continue;

                // Check if this token is an operator.
                if (OperatorDict.ContainsKey(token))
                {
                    var currentOperator = OperatorDict[token];
                    // Determine the type of Operator.
                    switch (currentOperator)
                    {
                        case GroupingOperator groupingOperator:
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
                            break;
                        case ArithmeticOperator arithmeticOperator:
                            // Determine the type of Arithmetic Operator.
                            if (arithmeticOperator.HighLevel)
                            {
                                // For high-level Arithmetic Operators, just add them to the stack.
                                operatorStack.Push(arithmeticOperator);
                            }
                            else
                            {
                                /* For non-high-level Arithmetic Operators, 
                                   we must first check for another non-high-level Arithmetic Operator
                                   at the top of the stack. */
                                if (IsArithmeticAtTop(operatorStack, false, true))
                                {
                                    ComputeTopOperatorWithTopValues(valueStack, operatorStack);
                                }

                                // Now we can add this operator to the stack.
                                operatorStack.Push(arithmeticOperator);
                            }
                            break;
                    }
                }
                else
                {
                    // Check if the token is a variable/value
                    if (ExpressionVariableRegex.IsMatch(token))
                    {
                        // This is a variable. Determine its true value and add it to the stack.
                        AddValueToStack(variableEvaluator(token), valueStack, operatorStack);
                    }
                    else if (int.TryParse(token, out var value))
                    {
                        // This is a normal integer. Add it to the stack.
                        AddValueToStack(value, valueStack, operatorStack);
                    }
                    else
                    {
                        // At this point, we have run out of options for parsing. Cannot parse this token.
                        throw new ArgumentException($"A token was not recognized as an operation or a value: {token}",
                            nameof(expression));
                    }
                }
            }

            // Clear out the last remaining non-high-level Arithmetic Operator if there is one.
            if (operatorStack.Count == 1 && IsArithmeticAtTop(operatorStack, false, true))
            {
                ComputeTopOperatorWithTopValues(valueStack, operatorStack);
            }

            // Check that we do not have any left-over Operators.
            if (operatorStack.Count != 0)
                throw new ArgumentException("There are too many operators left-over! Cannot determine a result.",
                    nameof(expression));

            // Check if there are no values.
            if (valueStack.Count == 0)
                throw new ArgumentException(
                    "There are no values to return. The expression may be empty!",
                    nameof(expression));

            // Check if there are too many values.
            if (valueStack.Count > 1)
                throw new ArgumentException(
                    $"There are too many values to return ({valueStack.Count}). The expression may be missing an operator!",
                    nameof(expression));

            // Finally, return the last remaining value.
            return valueStack.Pop();
        }

        /// <summary>
        /// Adds a new value to the top of the valueStack, and performs any needed computations.
        /// </summary>
        /// <param name="value">The value to add to the stack.</param>
        /// <param name="valueStack">The stack to add the value to.</param>
        /// <param name="operatorStack">The operator stack, used for computation of high-level arithmetic when required.</param>
        private static void AddValueToStack(int value, Stack<int> valueStack, Stack<Operator> operatorStack)
        {
            // Add the value to the stack.
            valueStack.Push(value);

            // Check for a high-level Arithmetic Operator at the top of the stack.
            if (IsArithmeticAtTop(operatorStack, true))
            {
                // Operator found. Perform computation.
                ComputeTopOperatorWithTopValues(valueStack, operatorStack);
            }
        }

        /// <summary>
        /// Performs the required operations on the value and operator stacks when an expression group is closed.
        /// </summary>
        /// <param name="valueStack">The stack containing integer values.</param>
        /// <param name="operatorStack">The stack containing the Operators to perform on the values.</param>
        private static void CloseGroup(Stack<int> valueStack, Stack<Operator> operatorStack)
        {
            // Check that there is an Arithmetic Operator to compute within the group.
            if (IsArithmeticAtTop(operatorStack))
            {
                // Perform computation.
                ComputeTopOperatorWithTopValues(valueStack, operatorStack);
            }

            // Check for a closing Group Operator to remove.
            if (operatorStack.Count == 0 ||
                !(operatorStack.Pop() is GroupingOperator groupingOperator && groupingOperator.OpensGroup))
            {
                throw new ArgumentException(
                    "Could not find an opening Group Operator to close.", nameof(operatorStack));
            }

            // Check if there is a high-level Arithmetic Operator that needs to be computed.
            if (IsArithmeticAtTop(operatorStack, true))
            {
                // Perform computation.
                ComputeTopOperatorWithTopValues(valueStack, operatorStack);
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
        private static void ComputeTopOperatorWithTopValues(Stack<int> valueStack, Stack<Operator> operatorStack)
        {
            if (!(operatorStack.Pop() is ArithmeticOperator arithmeticOperator))
                throw new ArgumentException(
                    "The Operator at the top of the stack was not an Arithmetic Operator.", nameof(operatorStack));

            // Pop two values and compute them against the operator. Store the result in the value stack.
            var values = PopTwoValues(valueStack);
            try
            {
                valueStack.Push(arithmeticOperator.Compute(values.Item2, values.Item1));
            }
            catch (DivideByZeroException e)
            {
                throw new ArgumentException("The expression contains an attempt to divide by zero.", e);
            }
        }

        /// <summary>
        /// Determines if the Operator at the top of the operatorStack is an Arithmetic Operator,
        /// and optionally, if it is high or low level.
        /// 
        /// (Note: if both highLevel and lowLevel are true, highLevel takes prescendence.)
        /// </summary>
        /// <param name="operatorStack">The Operator stack.</param>
        /// <param name="highLevel">Set to true to require that the Operator is high level.</param>
        /// <param name="lowLevel">Set to true to require that the Operator is low level.</param>
        /// <returns>True if the Operator at the top matches, false otherwise.</returns>
        private static bool IsArithmeticAtTop(Stack<Operator> operatorStack, bool highLevel = false,
            bool lowLevel = false)
        {
            // Check that we have any operators at all.
            if (operatorStack.Count == 0)
                return false;

            // Check if the type is correct.
            if (operatorStack.Peek() is ArithmeticOperator arithmeticOperator)
            {
                // If we don't care about high or low level, we have succeeded.
                if (!highLevel && !lowLevel)
                    return true;

                // Either highLevel or lowLevel is true, so determine which and return the result.
                return highLevel ? arithmeticOperator.HighLevel : arithmeticOperator.LowLevel;
            }

            // Not Arithmetic Operator at all.
            return false;
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
                throw new ArgumentException("There are not enough values to pop.", nameof(valueStack), e);
            }
        }
    }
}