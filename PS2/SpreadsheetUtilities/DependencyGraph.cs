using System.Collections.Generic;
using System.Linq;
using IPairDict = System.Collections.Generic.IDictionary<string, System.Collections.Generic.HashSet<string>>;
using PairDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>;


namespace SpreadsheetUtilities
{
    /// <summary>
    /// This DependencyGraph links string nodes together to form a dependency relationship consisting of dependees and dependents.
    /// <list type="bullet">
    ///     <item>
    ///     Asking for a node's set of dependents is asking: "who depends on me?", or "what must be calculated after this node?"
    ///     </item>
    ///     <item>
    ///     Asking for a node's set of dependees is asking: "who do I depend upon?", or "what must be calculated before this node?"
    ///     </item>
    /// </list>
    /// 
    /// <code>
    ///  For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </code>
    /// 
    ///  </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    public class DependencyGraph
    {
        /// <summary>
        /// Maps nodes to dependents.
        /// </summary>
        private readonly IPairDict _dependents = new PairDict();

        /// <summary>
        /// Maps nodes to dependees.
        /// </summary>
        private readonly IPairDict _dependees = new PairDict();

        /// <summary>
        /// The number of ordered pairs stored in this container.
        /// </summary>
        private int _count;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size => _count;

        /// <summary>
        /// Obtains the number of dependees for the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The number of dependees belonging to the given node.</returns>
        public int this[string node] => GetDependeesCount(node);


        /// <summary>
        /// Reports whether dependents(s) is non-empty for a given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node has dependents.</returns>
        public bool HasDependents(string node)
        {
            return GetDependentsCount(node) > 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty for a given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node has dependees.</returns>
        public bool HasDependees(string node)
        {
            return GetDependeesCount(node) > 0;
        }


        /// <summary>
        /// Gets an enumerable collection of dependents for a given node.
        /// </summary>
        /// <param name="node">The node to retreive dependents of.</param>
        /// <returns>An enumerable collection of the node's dependents.</returns>
        public IEnumerable<string> GetDependents(string node)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependents.TryGetValue(node, out var dependents) ? dependents.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependents to a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The count of dependents mapped.</returns>
        private int GetDependentsCount(string node)
        {
            return _dependents.TryGetValue(node, out var dependents) ? dependents.Count : 0;
        }

        /// <summary>
        /// Gets an enumerable collection of dependees for a given node.
        /// </summary>
        /// <param name="node">The node to retreive dependees of.</param>
        /// <returns>An enumerable collection of the node's dependees.</returns>
        public IEnumerable<string> GetDependees(string node)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependees.TryGetValue(node, out var dependees) ? dependees.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependees to a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The count of dependees mapped.</returns>
        private int GetDependeesCount(string node)
        {
            return _dependees.TryGetValue(node, out var dependees) ? dependees.Count : 0;
        }

        /// <summary>
        /// Adds an ordered pair dependency to the graph if it does not already exist.
        /// </summary>
        /// <param name="dependeeNode">The dependee; i.e. the node that the dependent node depends on.</param>
        /// <param name="dependentNode">The dependent; i.e. the node that depends on the dependee.</param>       
        public void AddDependency(string dependeeNode, string dependentNode)
        {
            // Make sure the keys exist.
            InitializeKey(dependeeNode, _dependents);
            InitializeKey(dependentNode, _dependees);

            // Create the relationships.
            var added = _dependents[dependeeNode].Add(dependentNode);
            added &= _dependees[dependentNode].Add(dependeeNode);

            // Increase count if the pair was added (might not be added if it already exists).
            if (added)
                _count++;
        }


        /// <summary>
        /// Removes an ordered pair dependency from the graph if it exists.
        /// </summary>
        /// <param name="dependeeNode">The dependee; i.e. the node that the dependent node depends on.</param>
        /// <param name="dependentNode">The dependent; i.e. the node that depends on the dependee.</param>   
        public void RemoveDependency(string dependeeNode, string dependentNode)
        {
            // Check if there is a key; if true, remove from the set if the value exists.
            var removed = false;
            if (_dependents.TryGetValue(dependeeNode, out var dependents))
                removed = dependents.Remove(dependentNode);
            if (_dependees.TryGetValue(dependentNode, out var dependees))
                removed &= dependees.Remove(dependeeNode);

            // Decrease count if the pair was removed (might not be removed if it did not exist).
            if (removed)
                _count--;
        }

        /// <summary>
        /// Removes all dependents for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void RemoveDependents(string node)
        {
            if (!_dependents.TryGetValue(node, out var dependents)) return;

            // Remove reverse side relationships
            foreach (var dependent in dependents)
                _dependees[dependent].Remove(node);

            // Update count
            _count -= dependents.Count;

            // Clear the dependents set.
            dependents.Clear();
        }

        /// <summary>
        /// Removes all dependees for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void RemoveDependees(string node)
        {
            if (!_dependees.TryGetValue(node, out var dependees)) return;

            // Remove reverse side relationships
            foreach (var dependee in dependees)
                _dependents[dependee].Remove(node);

            // Update count
            _count -= dependees.Count;

            // Clear the dependees set.
            dependees.Clear();
        }

        /// <summary>
        /// Replaces all dependents for a given node with the ones provided.
        /// </summary>
        /// <param name="node">The node to update.</param>
        /// <param name="newDependents">A collection containing the new dependents for the node.</param>
        public void ReplaceDependents(string node, IEnumerable<string> newDependents)
        {
            RemoveDependents(node);
            foreach (var dependent in newDependents)
                AddDependency(node, dependent);
        }


        /// <summary>
        /// Replaces all dependees for a given node with the ones provided.
        /// </summary>
        /// <param name="node">The node to update.</param>
        /// <param name="newDependees">A collection containing the new dependees for the node.</param>
        public void ReplaceDependees(string node, IEnumerable<string> newDependees)
        {
            RemoveDependees(node);
            foreach (var dependee in newDependees)
                AddDependency(dependee, node);
        }

        /// <summary>
        /// Checks if the key exists in the given Dictionary, and if not, maps it to an empty set.
        /// </summary>
        /// <param name="key">The key to initialize.</param>
        /// <param name="dict">The Dictionary to check for the key.</param>
        private static void InitializeKey(string key, IPairDict dict)
        {
            // Check if the key exists.
            if (dict.ContainsKey(key))
                return; // Return if it does.

            // Map the key to a new set.
            dict[key] = new HashSet<string>();
        }
    }
}