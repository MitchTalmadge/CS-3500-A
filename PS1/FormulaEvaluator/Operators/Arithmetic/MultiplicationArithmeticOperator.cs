using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluator.Operators.Arithmetic
{

    /// <summary>
    /// An Arithmetic Operator that performs multiplication.
    /// </summary>
    internal class MultiplicationArithmeticOperator : ArithmeticOperator
    {
        internal MultiplicationArithmeticOperator() : base(true)
        {
        }

        public override int Compute(int left, int right)
        {
            return left * right;
        }
    }
}