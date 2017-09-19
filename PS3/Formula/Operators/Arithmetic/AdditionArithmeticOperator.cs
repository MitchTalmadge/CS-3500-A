namespace SpreadsheetUtilities.Operators.Arithmetic
{
    /// <inheritdoc />
    /// <summary>
    /// An Arithmetic Operator that performs addition.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class AdditionArithmeticOperator : ArithmeticOperator
    {
        public override double Compute(double left, double right)
        {
            return left + right;
        }
    }
}