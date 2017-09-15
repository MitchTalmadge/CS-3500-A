using System.Collections.Generic;
using System.Linq;
using IPairDict = System.Collections.Generic.IDictionary<string, System.Collections.Generic.HashSet<string>>;
using PairDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// This class is a custom structure to represent ordered pairs in a Dependency Graph.
    /// The structure contains two dictionaries; one that links nodes to dependees, and one that links nodes to dependents. 
    /// In essence, this class acts like one dictionary that can be accessed in either direction; a many-to-many relationship container.
    /// 
    /// In this structure, an increase in memory usage (as a result of using two dictionaries) is exchanged for increased speeds 
    /// when retrieving relationships.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class DependencyContainer
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
        public int Count { get; private set; }

        /// <summary>
        /// Adds a pair to the container, skipping duplicates.
        /// </summary>
        /// <param name="dependentNode">The node that depends on the dependee.</param>
        /// <param name="dependeeNode">The node that is depended upon.</param>
        public void AddPair(string dependeeNode, string dependentNode)
        {
            // Make sure the keys exist.
            InitializeKey(dependeeNode, _dependents);
            InitializeKey(dependentNode, _dependees);

            // Create the relationships.
            var added = _dependents[dependeeNode].Add(dependentNode);
            added &= _dependees[dependentNode].Add(dependeeNode);

            // Increase count if the pair was added (might not be added if it already exists).
            if (added)
                Count++;
        }

        /// <summary>
        /// Removes a pair from the container if it exists.
        /// </summary>
        /// <param name="dependentNode">The node that depends on the dependee.</param>
        /// <param name="dependeeNode">The node that is depended upon.</param>
        public void RemovePair(string dependeeNode, string dependentNode)
        {
            // Check if there is a key; if true, remove from the set if the value exists.
            var removed = false;
            if (_dependents.TryGetValue(dependeeNode, out var dependents))
                removed = dependents.Remove(dependentNode);
            if (_dependees.TryGetValue(dependentNode, out var dependees))
                removed &= dependees.Remove(dependeeNode);

            // Decrease count if the pair was removed (might not be removed if it did not exist).
            if (removed)
                Count--;
        }

        /// <summary>
        /// Removes all dependents for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void RemoveDependents(string node)
        {
            if (!_dependents.TryGetValue(node, out var dependents)) return;

            // Remove reverse side relationships
            foreach (var dependent in dependents)
                _dependees[dependent].Remove(node);

            // Update count
            Count -= dependents.Count;

            // Clear the dependents set.
            dependents.Clear();
        }

        /// <summary>
        /// Removes all dependees for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void RemoveDependees(string node)
        {
            if (!_dependees.TryGetValue(node, out var dependees)) return;

            // Remove reverse side relationships
            foreach (var dependee in dependees)
                _dependents[dependee].Remove(node);

            // Update count
            Count -= dependees.Count;

            // Clear the dependees set.
            dependees.Clear();
        }

        /// <summary>
        /// Gets all mapped dependents for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An array containing the mapped dependents. May be of length 0 if none are mapped.</returns>
        public string[] GetDependents(string node)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependents.TryGetValue(node, out var dependents) ? dependents.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependents to a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The count of dependents mapped.</returns>
        public int GetDependentsCount(string node)
        {
            return _dependents.TryGetValue(node, out var dependents) ? dependents.Count : 0;
        }

        /// <summary>
        /// Gets all mapped dependees for a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An array containing the mapped dependees. May be of length 0 if none are mapped.</returns>
        public string[] GetDependees(string node)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependees.TryGetValue(node, out var dependees) ? dependees.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependees to a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The count of dependees mapped.</returns>
        public int GetDependeesCount(string node)
        {
            return _dependees.TryGetValue(node, out var dependees) ? dependees.Count : 0;
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