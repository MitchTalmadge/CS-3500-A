namespace SpreadsheetUtilities.Operators.Arithmetic
{
    /// <inheritdoc />
    /// <summary>
    /// An Arithmetic Operator that performs division.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class DivisionArithmeticOperator : ArithmeticOperator
    {
        internal DivisionArithmeticOperator() : base(true)
        {
        }

        public override double Compute(double left, double right)
        {
            return left / right;
        }
    }
}