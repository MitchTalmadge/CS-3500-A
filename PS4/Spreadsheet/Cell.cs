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
        public Cell(object content)
        {
            Content = content;
        }

        /// <summary>
        /// Represents the original, non-evaluated content of this Cell.
        /// May be a double, string, or Formula.
        /// </summary>
        internal object Content { get; }

        /// <summary>
        /// Evaluates the content of the cell. 
        /// Strings and doubles return themselves, Formula returns its evaluated double value or a FormulaError.
        /// </summary>
        /// <param name="lookup">The lookup to use when evaluating Formulas.</param>
        /// <returns>The evaluated content of the cell.</returns>
        public object Evaluate(Func<string, double> lookup)
        {
            // Check if the content is a formula, which must be evaluated with the Formula#Evaluate method.
            if (Content is Formula formula)
                return formula.Evaluate(lookup);

            // Otherwise, doubles and strings can simply be returned.
            return Content;
        }
    }
}