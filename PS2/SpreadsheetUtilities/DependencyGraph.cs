// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </summary>
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
            _dependencies.AddPair(dependentNode, dependeeNode);
        }


        /// <summary>
        /// Removes an ordered pair dependency from the graph if it exists.
        /// </summary>
        /// <param name="dependeeNode">The dependee; i.e. the node that the dependent node depends on.</param>
        /// <param name="dependentNode">The dependent; i.e. the node that depends on the dependee.</param>   
        public void RemoveDependency(string dependeeNode, string dependentNode)
        {
            _dependencies.RemovePair(dependentNode, dependeeNode);
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
                _dependencies.AddPair(dependent, node);
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
                _dependencies.AddPair(node, dependee);
        }

    }

}