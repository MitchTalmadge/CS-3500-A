using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;

namespace SS
{
    /// <inheritdoc />
    /// <summary>
    /// An implementation of the AbstractSpreadsheet.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Maps the names of cells to their instances.
        /// Any cells not in this table are considered "empty" and contain only an empty string ("").
        /// </summary>
        private readonly Dictionary<string, Cell> _cells = new Dictionary<string, Cell>();
        
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Copy the cells to a new array.
            var cells = new string[_cells.Keys.Count];
            _cells.Keys.CopyTo(cells, 0);

            // Return the copy.
            return cells;
        }

        public override object GetCellContents(string name)
        {
            if (!_cells.TryGetValue(name, out var cell))
                return "";

            return cell.Content;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            if(name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            return null;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            return null;
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            return null;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return null;
        }

        /// <summary>
        /// Determines if a given cell name is considered syntactically valid.
        /// Validity means it starts with a letter or underscore, and is followed by 0 or more letters, numbers, or underscores.
        /// </summary>
        /// <param name="name">The name to parse.</param>
        /// <returns>True if the name is valid.</returns>
        private static bool IsCellNameValid(string name)
        {
            const string variablePattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
            return Regex.IsMatch(name, variablePattern, RegexOptions.IgnorePatternWhitespace);
        }
    }
}
