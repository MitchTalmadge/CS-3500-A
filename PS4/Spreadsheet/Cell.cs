using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}