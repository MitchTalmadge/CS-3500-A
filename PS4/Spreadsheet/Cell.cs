using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// Represents a single cell in a spreadsheet.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class Cell
    {
        /// <summary>
        /// Constructs a Cell with the given contents.
        /// </summary>
        /// <param name="content">
        /// The original contents of the Cell (not evaluated).
        /// Should be either a double, string, or Formula.
        /// </param>
        internal Cell(object content)
        {
            Content = content;
        }

        /// <summary>
        /// Represents the original, non-evaluated content of this Cell.
        /// May be a double, string, or Formula.
        /// </summary>
        internal object Content { get; }

        internal object Value;

        /// <summary>
        /// Gets the value of this cell. 
        /// If the value has not been calculated yet, will perform calculations and save the value 
        /// for quicker access next time.
        /// </summary>
        /// <param name="lookup">The lookup to use when evaluating Formulas.</param>
        /// <returns>The value of this cell.</returns>
        internal object GetValue(Func<string, double> lookup)
        {
            // Return stored value if not null.
            if (Value != null)
                return Value;

            // Otherwise, recalculate.
            CalculateValue(lookup);
            return Value;
        }

        /// <summary>
        /// Clears the value of this cell so that it will be re-calculated next time it is requested.
        /// </summary>
        internal void ClearValue()
        {
            Value = null;
        }

        /// <summary>
        /// Calculates the value of this cell and stores it in the Value field.
        /// </summary>
        /// <param name="lookup">The lookup to use when evaluating Formulas.</param>
        private void CalculateValue(Func<string, double> lookup)
        {
            // Check if the content is a formula, which must be evaluated with the Formula#Evaluate method.
            if (Content is Formula formula)
                Value = formula.Evaluate(lookup);

            // Doubles and strings are simply themselves as values.
            Value = Content;
        }
    }
}