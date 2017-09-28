using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// This dependency graph links the cells which contain formulas together.
        /// </summary>
        private readonly DependencyGraph _formulaCellDependencyGraph = new DependencyGraph();

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
            // Check name validity
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            // Check if the cell has any contents
            return !_cells.TryGetValue(name, out var cell) ? "" : cell.Content;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            return UpdateCell(name, number);
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            // Check name validity
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            // Text cannot be null.
            if (text == null)
                throw new ArgumentNullException();

            return UpdateCell(name, text);
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !IsCellNameValid(name))
                throw new InvalidNameException();

            if (formula == null)
                throw new ArgumentNullException();

            return UpdateCell(name, formula);
        }

        /// <summary>
        /// Updates a cell with the given contents.
        /// Updates formula dependencies where needed.
        /// </summary>
        /// <param name="name">The name of the cell to update.</param>
        /// <param name="contents">The new contents of the cell.</param>
        /// <returns>The cells which depend on the updated cell, either directly or indirectly.</returns>
        private ISet<string> UpdateCell(string name, object contents)
        {
            // Dependencies of formulas must be added to the graph.
            if (contents is Formula formula)
            {
                // Add dependencies to graph.
                foreach (var variable in formula.GetVariables())
                {
                    _formulaCellDependencyGraph.AddDependency(variable, name);
                }
            }

            // Check cells to recalculate, also catching circular dependencies.
            var cellsToRecalculate = new HashSet<string>(GetCellsToRecalculate(name));

            // Remove ourselves from the cells to recalculate.
            cellsToRecalculate.Remove(name);

            // Text cells must be checked carefully because setting a cell to empty text removes it from the dictionary.
            if (contents is string text && text == "")
            {
                _cells.Remove(name);
                return cellsToRecalculate;
            }

            _cells[name] = new Cell(contents);
            return cellsToRecalculate;
        }

        /// <inheritdoc />
        /// <summary>
        /// Uses the dependency graph to find dependent cells.
        /// </summary>
        /// <param name="name">The name of the cell of which to find dependents.</param>
        /// <returns>The direct dependents of the given cell, if any.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            return _formulaCellDependencyGraph.GetDependents(name);
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