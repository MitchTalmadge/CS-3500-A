namespace FormulaEvaluator.Operators.Arithmetic
{
    abstract class ArithmeticOperator : Operator
    {
        /// <summary>
        /// Determines if this Arithmetic Operator is considered "high level",
        /// or in other words, should be computed before anything else.
        /// </summary>
        public bool HighLevel { get; }

        /// <inheritdoc />
        /// <summary>
        /// Constructs an Arithmetic Operator with the given token,
        /// and with the option to be marked as high level.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="highLevel"></param>
        protected ArithmeticOperator(string token, bool highLevel = false) : base(token)
        {
            HighLevel = highLevel;
        }

        /// <summary>
        /// Computes the result of applying this operation between the left and right parameters.
        /// For example, if the operator is "add": left + right = result
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The computed value.</returns>
        public abstract int Compute(int left, int right);
    }
}