using System.Collections.Generic;
using System.Linq;
using IPairDict = System.Collections.Generic.IDictionary<string, System.Collections.Generic.HashSet<string>>;
using PairDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<string>>;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// This class is a custom structure to represent ordered pairs in a Dependency Graph.
    /// The structure contains two dictionaries; one that links dependents to dependees, and one that links dependees to dependents. 
    /// In essence, this class acts like one dictionary that can be accessed in either direction; a many-to-many relationship container.
    /// 
    /// In this structure, an increase in memory usage (as a result of using two dictionaries)
    /// is exchanged for increased speeds when searching for relationships.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    internal class DependencyContainer
    {
        /// <summary>
        /// The "forward" Dictionary, which maps dependents to dependees.
        /// </summary>
        private readonly IPairDict _dependents = new PairDict();

        /// <summary>
        /// The "reverse" Dictionary, which maps dependees to dependents.
        /// </summary>
        private readonly IPairDict _dependees = new PairDict();

        /// <summary>
        /// The number of ordered pairs stored in this container.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Adds a pair to the container, skipping duplicates.
        /// </summary>
        /// <param name="dependent">The node that depends on the dependee.</param>
        /// <param name="dependee">The node that is depended upon.</param>
        public void AddPair(string dependent, string dependee)
        {
            // Make sure the keys exist.
            InitializeKey(dependent, _dependents);
            InitializeKey(dependee, _dependees);

            // Create the relationships.
            var added = _dependents[dependent].Add(dependee);
            added &= _dependees[dependee].Add(dependent);

            // Increase count if the pair was added (might not be added if it already exists).
            if (added)
                Count++;
        }

        /// <summary>
        /// Removes a pair from the container if it exists.
        /// </summary>
        /// <param name="dependent">The node that depends on the dependee.</param>
        /// <param name="dependee">The node that is depended upon.</param>
        public void RemovePair(string dependent, string dependee)
        {
            // Check if there is a key; if true, remove from the set if the value exists.
            var removed = false;
            if (_dependents.TryGetValue(dependent, out var dependees))
                removed = dependees.Remove(dependee);
            if (_dependees.TryGetValue(dependee, out var dependents))
                removed &= dependents.Remove(dependent);

            // Decrease count if the pair was removed (might not be removed if it did not exist).
            if (removed)
                Count--;
        }

        /// <summary>
        /// Removes all dependents for a dependee.
        /// </summary>
        /// <param name="dependee">The dependee.</param>
        public void RemoveDependents(string dependee)
        {
            if (!_dependees.TryGetValue(dependee, out var dependents)) return;

            // Remove reverse side relationships
            foreach (var dependent in dependents)
                _dependents[dependent].Remove(dependee);

            // Update count
            Count -= dependents.Count;

            // Clear the dependents set.
            dependents.Clear();
        }

        /// <summary>
        /// Removes all dependees for a dependent.
        /// </summary>
        /// <param name="dependent">The dependent.</param>
        public void RemoveDependees(string dependent)
        {
            if (!_dependents.TryGetValue(dependent, out var dependees)) return;

            // Remove reverse side relationships
            foreach (var dependee in dependees)
                _dependees[dependee].Remove(dependent);

            // Update count
            Count -= dependees.Count;

            // Clear the dependees set.
            dependees.Clear();
        }

        /// <summary>
        /// Gets all mapped dependents for a dependee.
        /// </summary>
        /// <param name="dependee">The dependee.</param>
        /// <returns>An array containing the mapped dependents. May be of length 0 if none are mapped.</returns>
        public string[] GetDependents(string dependee)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependees.TryGetValue(dependee, out var dependents) ? dependents.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependents to a dependee.
        /// </summary>
        /// <param name="dependee">The dependee.</param>
        /// <returns>The count of dependents mapped.</returns>
        public int GetDependentsCount(string dependee)
        {
            return _dependees.TryGetValue(dependee, out var dependents) ? dependents.Count : 0;
        }

        /// <summary>
        /// Gets all mapped dependees for a dependent.
        /// </summary>
        /// <param name="dependent">The dependent.</param>
        /// <returns>An array containing the mapped dependees. May be of length 0 if none are mapped.</returns>
        public string[] GetDependees(string dependent)
        {
            // Returns the contents of the dictionary at the given key as an array (to prevent modifications) if the key exists, or an empty array if it does not.
            return _dependents.TryGetValue(dependent, out var dependees) ? dependees.ToArray() : new string[0];
        }

        /// <summary>
        /// Gets the number of mapped dependees to a dependent.
        /// </summary>
        /// <param name="dependent">The dependent.</param>
        /// <returns>The count of dependees mapped.</returns>
        public int GetDependeesCount(string dependent)
        {
            return _dependents.TryGetValue(dependent, out var dependees) ? dependees.Count : 0;
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