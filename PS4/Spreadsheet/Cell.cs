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
        public Cell(object content)
        {
            Content = content;
        }

        internal object Content { get; }
    }
}