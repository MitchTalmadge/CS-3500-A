namespace FormulaEvaluator.Operators
{
    /// <summary>
    /// Represents an operator in an expression as accepted by the Evaluator. 
    /// </summary>
    public abstract class Operator
    {
        /// <summary>
        /// The token representation of the operator (how it would be displayed in an expression).
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Constructs an Operator with the given token.
        /// </summary>
        /// <param name="token">The token representation of the operator (how it would be displayed in an expression).</param>
        protected Operator(string token)
        {
            Token = token;
        }
    }
}