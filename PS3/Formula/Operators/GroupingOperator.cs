namespace SpreadsheetUtilities.Operators
{
    /// <inheritdoc />
    /// <summary>
    /// Grouping Operators are used to "group" parts of expressions together. Most commonly, they would be parentheses.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class GroupingOperator : Operator
    {
        /// <summary>
        /// Determines if this is the left brace, which opens the group.
        /// </summary>
        public bool OpensGroup { get; }

        /// <summary>
        /// Constructs a Grouping Operator.
        /// </summary>
        /// <param name="closeGroup">
        /// Whether this operator closes the group. 
        /// Defaults to false, meaning that this operator opens the group.
        /// </param>
        public GroupingOperator(bool closeGroup = false)
        {
            OpensGroup = !closeGroup;
        }
    }
}