using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
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

        public override bool Changed { get; protected set; }

        public Spreadsheet() : this(s => true, s => s, "default")
        {
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid,
            normalize, version)
        {
        }

        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            LoadFile(filePath, version);
        }

        /// <summary>
        /// Loads the spreadsheet file at the provided location.
        /// Compares the version within the file to the provided version.
        /// If anything goes wrong, throws a SpreadsheetReadWriteException.
        /// </summary>
        /// <param name="filePath">The path to the file to load.</param>
        /// <param name="expectedVersion">The spreadsheet version that is expected in the file.</param>
        private void LoadFile(string filePath, string expectedVersion)
        {
            try
            {
                // Make sure version of file matches version provided
                if (GetSavedVersion(filePath) != expectedVersion)
                {
                    throw new SpreadsheetReadWriteException(
                        "The provided version does not match the version of the passed in file.");
                }

                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true,
                    IgnoreComments = true
                };

                // Load the file with the XmlReader
                using (var reader = XmlReader.Create(filePath, settings))
                {
                    // Start element = <spreadsheet>
                    reader.Read();

                    // Read spreadsheet's children.
                    while (reader.Read())
                    {
                        // Check for closing spreadsheet tag.
                        if (reader.Name == "spreadsheet" && reader.NodeType == XmlNodeType.EndElement)
                            break;

                        // Make sure it's a cell.
                        if (reader.Name != "cell")
                            throw new SpreadsheetReadWriteException(
                                "Found an element within <spreadsheet> that is not a <cell>!");

                        // Read and validate name.
                        reader.Read();
                        if (reader.Name != "name")
                            throw new SpreadsheetReadWriteException("The first element under a <cell> was not <name>!");
                        var name = reader.ReadElementContentAsString();

                        if (reader.Name != "contents")
                            throw new SpreadsheetReadWriteException(
                                "The second element under <cell> was not <contents> for the cell: " + name);
                        var content = reader.ReadElementContentAsString();

                        // Check for closing tag.
                        if (reader.Name != "cell" || reader.NodeType != XmlNodeType.EndElement)
                            throw new SpreadsheetReadWriteException(
                                "A closing </cell> tag was not found for the cell: " + name);

                        // Try to add the cell to the spreadsheet.
                        try
                        {
                            SetContentsOfCell(name, content);
                        }
                        catch (InvalidNameException)
                        {
                            throw new SpreadsheetReadWriteException("A cell had an invalid name: " + name);
                        }
                        catch (CircularException)
                        {
                            throw new SpreadsheetReadWriteException(
                                "There is a circular reference in the spreadsheet at the cell: " + name);
                        }
                        catch (FormulaFormatException)
                        {
                            throw new SpreadsheetReadWriteException(
                                "The Formula could not be parsed for the cell: " + name);
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                throw new SpreadsheetReadWriteException("An unknown error occurred while reading the spreadsheet: " +
                                                        e.Message);
            }
        }

        public override string GetSavedVersion(string filePath)
        {
            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            // Load the file with the XmlReader
            using (var reader = XmlReader.Create(filePath, settings))
            {
                // Start element = <spreadsheet>
                if (!reader.Read())
                    throw new SpreadsheetReadWriteException("The file is empty!");

                // Make sure the element is correct.
                if (reader.Name != "spreadsheet")
                    throw new SpreadsheetReadWriteException("The first element of the file is not <spreadsheet>!");

                // Make sure the version exists.
                string version;
                if ((version = reader.GetAttribute("version")) == null)
                    throw new SpreadsheetReadWriteException("The version attribute of <spreadsheet> is missing!");

                return version;
            }
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            if (name == null)
                throw new InvalidNameException();

            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            // Return the cell's value if it's in the dictionary, or "" if it's not.
            return _cells.TryGetValue(name, out var cell) ? cell.Value : "";
        }

        /// <summary>
        /// "Looks up" a cell's value and returns it as a double. Used when evaluating cells.
        /// If the cell evaluates to a string or FormulaError, will throw an ArgumentException.
        /// </summary>
        /// <param name="name">The name of the cell to look up.</param>
        /// <returns>The double value of the cell provided.</returns>
        private double LookupValue(string name)
        {
            // If the cell is not in the dictionary, it would be an empty string. Strings are not allowed.
            if (!_cells.TryGetValue(name, out var cell))
                throw new ArgumentException();

            // Get the cell's value.
            var value = cell.Value;

            // Strings and errors are not allowed.
            if (value is string || value is FormulaError)
                throw new ArgumentException();

            // If it is not a string, it must be a double.
            return (double) value;
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Copy the cells to a new array.
            var cells = new string[_cells.Keys.Count];
            _cells.Keys.CopyTo(cells, 0);

            // Return the copy.
            return cells;
        }

        /// <inheritdoc />
        public override object GetCellContents(string name)
        {
            if (name == null)
                throw new InvalidNameException();

            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            // Check if the cell has any contents
            return !_cells.TryGetValue(name, out var cell) ? "" : cell.Content;
        }

        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // Check null values
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if (name == null)
                throw new InvalidNameException();

            // Check normalized name for validity.
            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            // Double parse
            if (double.TryParse(content, out var parsed))
            {
                return SetCellContents(name, parsed);
            }

            // Formula parse
            if (content.StartsWith("="))
            {
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsCellNameValid));
            }

            // Anything else is string
            return SetCellContents(name, content);
        }

        /// <inheritdoc />
        protected override ISet<string> SetCellContents(string name, double number)
        {
            // Check null values
            if (name == null)
                throw new InvalidNameException();

            // Check normalized name for validity.
            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            return UpdateCell(name, number);
        }

        /// <inheritdoc />
        protected override ISet<string> SetCellContents(string name, string text)
        {
            // Check null values
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (name == null)
                throw new InvalidNameException();

            // Check normalized name for validity.
            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

            return UpdateCell(name, text);
        }

        /// <inheritdoc />
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // Check null values
            if (formula == null)
                throw new ArgumentNullException(nameof(formula));
            if (name == null)
                throw new InvalidNameException();

            // Check normalized name for validity.
            name = Normalize(name);
            if (!IsCellNameValid(name))
                throw new InvalidNameException();

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
            // Holds the cells which need to be recalculated.
            ISet<string> cellsToRecalculate;

            // Dependencies of formulas must be updated in the graph.
            if (contents is Formula formula)
            {
                // Keep track of the old dependees
                var oldDependees = _formulaCellDependencyGraph.GetDependees(name);

                // Replace the old dependees with the new dependees.
                _formulaCellDependencyGraph.ReplaceDependees(name, formula.GetVariables());

                // Check for a circular dependency.
                try
                {
                    cellsToRecalculate = new HashSet<string>(GetCellsToRecalculate(name));
                }
                catch (CircularException)
                {
                    // Restore old dependees upon a circular exception so that nothing is modified.
                    _formulaCellDependencyGraph.ReplaceDependees(name, oldDependees);
                    throw;
                }
            }
            else
            {
                // Clear any old dependees in the graph
                _formulaCellDependencyGraph.ReplaceDependees(name, new string[0]);

                // Find cells to re-calculate.
                cellsToRecalculate = new HashSet<string>(GetCellsToRecalculate(name));
            }

            if (contents is string text && text == "")
            {
                // If text is empty, the cell should be removed from the dictionary.
                _cells.Remove(name);
            }
            else
            {
                // Add a new cell to the dictionary, or update the existing cell.
                if (_cells.TryGetValue(name, out var cell))
                    cell.Content = contents;
                else
                    _cells[name] = new Cell(contents, LookupValue);
            }

            RecalculateCells(cellsToRecalculate);
            return cellsToRecalculate;
        }

        /// <summary>
        /// Recalculates all the cells with the names in the provided IEnumerable. 
        /// </summary>
        /// <param name="cellNames">The names of the cells to recalculate, in order of calculation.</param>
        private void RecalculateCells(IEnumerable<string> cellNames)
        {
            foreach (var cellName in cellNames)
            {
                if (_cells.TryGetValue(cellName, out var cell))
                    cell.RecalculateValue();
            }
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
        /// Determines if a given cell name is considered valid both syntactically and via the IsValid delegate.
        /// </summary>
        /// <param name="name">The name to parse. Important: Should be normalized first!</param>
        /// <returns>True if the name is valid.</returns>
        private bool IsCellNameValid(string name)
        {
            const string variablePattern = @"^[A-Z]+\d+$";
            return Regex.IsMatch(name, variablePattern, RegexOptions.IgnoreCase) && IsValid(name);
        }
    }
}