using System.Collections.Generic;

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
        /// Contains the ordered pairs for the dependency graph.
        /// </summary>
        private readonly DependencyContainer _dependencies = new DependencyContainer();

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size => _dependencies.Count;

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s] => _dependencies.GetDependeesCount(s);


        /// <summary>
        /// Reports whether dependents(s) is non-empty for a given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node has dependents.</returns>
        public bool HasDependents(string node)
        {
            return _dependencies.GetDependentsCount(node) > 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty for a given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node has dependees.</returns>
        public bool HasDependees(string node)
        {
            return _dependencies.GetDependeesCount(node) > 0;
        }


        /// <summary>
        /// Gets an enumerable collection of dependents for a given node.
        /// </summary>
        /// <param name="node">The node to retreive dependents of.</param>
        /// <returns>An enumerable collection of the node's dependents.</returns>
        public IEnumerable<string> GetDependents(string node)
        {
            return _dependencies.GetDependents(node);
        }

        /// <summary>
        /// Gets an enumerable collection of dependees for a given node.
        /// </summary>
        /// <param name="node">The node to retreive dependees of.</param>
        /// <returns>An enumerable collection of the node's dependees.</returns>
        public IEnumerable<string> GetDependees(string node)
        {
            return _dependencies.GetDependees(node);
        }

        /// <summary>
        /// Adds an ordered pair dependency to the graph if it does not already exist.
        /// </summary>
        /// <param name="dependeeNode">The dependee; i.e. the node that the dependent node depends on.</param>
        /// <param name="dependentNode">The dependent; i.e. the node that depends on the dependee.</param>       
        public void AddDependency(string dependeeNode, string dependentNode)
        {
            _dependencies.AddPair(dependeeNode, dependentNode);
        }


        /// <summary>
        /// Removes an ordered pair dependency from the graph if it exists.
        /// </summary>
        /// <param name="dependeeNode">The dependee; i.e. the node that the dependent node depends on.</param>
        /// <param name="dependentNode">The dependent; i.e. the node that depends on the dependee.</param>   
        public void RemoveDependency(string dependeeNode, string dependentNode)
        {
            _dependencies.RemovePair(dependeeNode, dependentNode);
        }


        /// <summary>
        /// Replaces all dependents for a given node with the ones provided.
        /// </summary>
        /// <param name="node">The node to update.</param>
        /// <param name="newDependents">A collection containing the new dependents for the node.</param>
        public void ReplaceDependents(string node, IEnumerable<string> newDependents)
        {
            _dependencies.RemoveDependents(node);
            foreach (var dependent in newDependents)
                _dependencies.AddPair(node, dependent);
        }


        /// <summary>
        /// Replaces all dependees for a given node with the ones provided.
        /// </summary>
        /// <param name="node">The node to update.</param>
        /// <param name="newDependees">A collection containing the new dependees for the node.</param>
        public void ReplaceDependees(string node, IEnumerable<string> newDependees)
        {
            _dependencies.RemoveDependees(node);
            foreach (var dependee in newDependees)
                _dependencies.AddPair(dependee, node);
        }
    }
}