using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities.Operators;
using SpreadsheetUtilities.Operators.Arithmetic;
using SpreadsheetUtilities.Utils;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Used for evaluation of expressions parsed by the Formula class.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class Evaluator
    {
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
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public static object Evaluate(string expression, Func<string, string> normalizer, Func<string, double> lookup)
        {
            // Split the expression up into individual tokens.
            var tokens = ExpressionUtils.GetTokens(expression);

            // The value stack contains the actual integer values to perform operations on.
            var valueStack = new Stack<double>();

            // The operator stack contains the Operators that will be applied to the values in the value stack: +, -, /, *, etc.
            var operatorStack = new Stack<Operator>();

            // A large try-catch is used to catch the EvaluationExceptions thrown by helper methods, which contain the FormulaError to return.
            try
            {
                // Iterate over each token to determine its type and how it should be handled.
                foreach (var token in tokens)
                {
                    // Check if this token is an operator.
                    if (OperatorDict.ContainsKey(token))
                    {
                        // Get the correct Operator from the dictionary by its token.
                        var currentOperator = OperatorDict[token];

                        // Determine the type of Operator.
                        switch (currentOperator)
                        {
                            case GroupingOperator groupingOperator:
                                // This is a Grouping Operator, i.e. parentheses. 

                                // Check if the grouping operator opens or closes the group.
                                if (groupingOperator.OpensGroup)
                                    // Opens the group. Simply add to the stack.
                                    operatorStack.Push(groupingOperator);
                                else
                                    // Closes the group. 
                                    CloseGroup(valueStack, operatorStack);
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
                                        ComputeTopOperatorWithTopValues(valueStack, operatorStack);

                                    // Now we can add this operator to the stack.
                                    operatorStack.Push(arithmeticOperator);
                                }
                                break;
                        }
                    }
                    else
                    {
                        // Check if the token is a variable/value
                        if (ExpressionUtils.IsVariable(token))
                            // This is a variable. Determine its true value and add it to the stack.
                            AddValueToStack(lookup(token), valueStack, operatorStack);
                        else if (double.TryParse(token, out var value))
                            // This is a normal integer. Add it to the stack.
                            AddValueToStack(value, valueStack, operatorStack);
                    }
                }

                // Clear out the last remaining non-high-level Arithmetic Operator if there is one.
                if (operatorStack.Count == 1 && IsArithmeticAtTop(operatorStack, false, true))
                    ComputeTopOperatorWithTopValues(valueStack, operatorStack);

                // Finally, return the last remaining value.
                return valueStack.Pop();
            }
            catch (EvaluationException e)
            {
                return e.Error;
            }
        }

        /// <summary>
        /// Adds a new value to the top of the valueStack, and performs any needed computations.
        /// </summary>
        /// <param name="value">The value to add to the stack.</param>
        /// <param name="valueStack">The stack to add the value to.</param>
        /// <param name="operatorStack">The operator stack, used for computation of high-level arithmetic when required.</param>
        private static void AddValueToStack(double value, Stack<double> valueStack, Stack<Operator> operatorStack)
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
        private static void CloseGroup(Stack<double> valueStack, Stack<Operator> operatorStack)
        {
            // Check that there is an Arithmetic Operator to compute within the group.
            if (IsArithmeticAtTop(operatorStack))
            {
                // Perform computation.
                ComputeTopOperatorWithTopValues(valueStack, operatorStack);
            }

            // Pop the closing group operator.
            operatorStack.Pop();

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
        private static void ComputeTopOperatorWithTopValues(Stack<double> valueStack, Stack<Operator> operatorStack)
        {
            var arithmeticOperator = (ArithmeticOperator) operatorStack.Pop();

            // Pop two values and compute them against the operator. Store the result in the value stack.
            var values = PopTwoValues(valueStack);
            try
            {
                valueStack.Push(arithmeticOperator.Compute(values.Item2, values.Item1));
            }
            catch (DivideByZeroException)
            {
                throw new EvaluationException(new FormulaError("Cannot divide by zero!"));
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
            if (!(operatorStack.Peek() is ArithmeticOperator arithmeticOperator))
                return false;

            // If we don't care about high or low level, we have succeeded.
            if (!highLevel && !lowLevel)
                return true;

            // Either highLevel or lowLevel is true, so determine which and return the result.
            return highLevel ? arithmeticOperator.HighLevel : arithmeticOperator.LowLevel;

            // Not Arithmetic Operator at all.
        }

        /// <summary>
        /// Pops two values from the top of the value stack and returns them in a Tuple.
        /// The first value popped is item1, and the second is item2.
        /// </summary>
        /// <param name="valueStack">The stack containing at least two values.</param>
        /// <returns>A Tuple containing the two values popped.</returns>
        private static Tuple<double, double> PopTwoValues(Stack<double> valueStack)
        {
            return new Tuple<double, double>(valueStack.Pop(), valueStack.Pop());
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// This exception is used as a convenient way to propagate a FormulaError up through the helper methods of the Evaluator class.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class EvaluationException : Exception
    {
        /// <summary>
        /// The error belonging to this exception.
        /// </summary>
        internal FormulaError Error { get; }

        /// <inheritdoc />
        /// <summary>
        /// Constructs an EvaluationException with the given FormulaError.
        /// </summary>
        /// <param name="error">The error to store in this exception.</param>
        public EvaluationException(FormulaError error)
        {
            Error = error;
        }
    }
}