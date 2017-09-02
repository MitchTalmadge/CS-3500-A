namespace FormulaEvaluator.Operators.Arithmetic
{
    internal abstract class ArithmeticOperator : Operator
    {
        /// <summary>
        /// Determines if this Arithmetic Operator is considered "high-level",
        /// or in other words, should be computed before other operators.
        /// </summary>
        public bool HighLevel { get; }

        /// <summary>
        /// Determines if this Arithmetic Operator is considered "low-level",
        /// or in other words, will be computed last; after other high-level operators.
        /// </summary>
        public bool LowLevel => !HighLevel;

        /// <inheritdoc />
        /// <summary>
        /// Constructs an Arithmetic Operator with the given token,
        /// </summary>
        /// <param name="highLevel">
        /// Whether or not this Arithmetic Operator is high-level, 
        /// meaning that it should be computed before other operators.
        /// </param>
        protected ArithmeticOperator(bool highLevel = false)
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