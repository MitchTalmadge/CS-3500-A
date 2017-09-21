namespace SpreadsheetUtilities.Operators.Arithmetic
{
    /// <inheritdoc />
    /// <summary>
    ///     An Arithmetic Operator that performs multiplication.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class MultiplicationArithmeticOperator : ArithmeticOperator
    {
        internal MultiplicationArithmeticOperator() : base(true)
        {
        }

        public override double Compute(double left, double right)
        {
            return left * right;
        }
    }
}