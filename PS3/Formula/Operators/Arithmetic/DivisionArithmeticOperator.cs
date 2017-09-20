using System;

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
            // Make sure we don't divide by zero.
            if(right.Equals(0.0))
                throw new DivideByZeroException();

            return left / right;
        }
    }
}