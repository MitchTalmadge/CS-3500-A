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
        /// Constructs a Cell with the given contents,
        /// and using the given lookup for evaluation.
        /// </summary>
        /// <param name="content">
        /// The original contents of the Cell (not evaluated).
        /// Should be either a double, string, or Formula.
        /// </param>
        /// <param name="lookup">The lookup to use when evaluating Formulas.</param>
        internal Cell(object content, Func<string, double> lookup)
        {
            Content = content;
            _lookup = lookup;
        }

        /// <summary>
        /// Represents the original, non-evaluated content of this Cell.
        /// May be a double, string, or Formula.
        /// </summary>
        internal object Content { get; set; }

        /// <summary>
        /// Represents the evaluated content of this Cell.
        /// May be a double, string, or FormulaError.
        /// </summary>
        internal object Value { get; private set; }

        /// <summary>
        /// The lookup to use when evaluating Formulas.
        /// </summary>
        private readonly Func<string, double> _lookup;

        /// <summary>
        /// Re-calculates the value of this cell.
        /// Use this when dependencies of the cell change.
        /// </summary>
        internal void RecalculateValue()
        {
            // Check if the content is a formula, which must be evaluated with the Formula#Evaluate method.
            if (Content is Formula formula)
                Value = formula.Evaluate(_lookup);

            // Doubles and strings are simply themselves as values.
            Value = Content;
        }
    }
}