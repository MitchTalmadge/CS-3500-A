using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluator.Operators.Arithmetic
{
    /// <summary>
    /// An Arithmetic Operator that performs addition.
    /// </summary>
    internal class AdditionArithmeticOperator : ArithmeticOperator
    {
        public override int Compute(int left, int right)
        {
            return left + right;
        }
    }
}