using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluator.Operators.Arithmetic
{

    /// <summary>
    /// An Arithmetic Operator that performs division.
    /// </summary>
    class DivisionArithmeticOperator : ArithmeticOperator
    {
        public DivisionArithmeticOperator() : base("/", true)
        {
        }

        public override int Compute(int left, int right)
        {
            return left / right;
        }
    }
}