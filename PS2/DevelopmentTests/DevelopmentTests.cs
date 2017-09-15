using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace DevelopmentTests
{
    /// <summary>
    /// Tests for the Dependency Graph implementation.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    [TestClass]
    public class DevelopmentTests
    {
        /// <summary>
        /// Tests that a newly created graph has a size of 0.
        /// </summary>
        [TestMethod]
        public void TestNewGraphHasSizeZero()
        {
            Assert.AreEqual(0, new DependencyGraph().Size);
        }

        /// <summary>
        /// Tests that a newly created graph does not contain dependees for any given node.
        /// </summary>
        [TestMethod]
        public void TestNewGraphHasNoDependees()
        {
            Assert.IsFalse(new DependencyGraph().HasDependees("a"));
            Assert.IsFalse(new DependencyGraph().GetDependees("a").GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Tests that a newly created graph does not contain dependents for any given node.
        /// </summary>
        [TestMethod]
        public void TestNewGraphHasNoDependents()
        {
            Assert.IsFalse(new DependencyGraph().HasDependents("a"));
            Assert.IsFalse(new DependencyGraph().GetDependents("a").GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Tests that a newly created graph has a dependees size of 0 for any given node when using the indexer.
        /// </summary>
        [TestMethod]
        public void TestNewGraphHasIndexerSizeZero()
        {
            Assert.AreEqual(0, new DependencyGraph()["a"]);
        }

        /// <summary>
        /// Tests removing a non-existent dependency from a newly created (empty) graph.
        /// </summary>
        [TestMethod]
        public void TestRemoveFromEmptyGraph()
        {
            var graph = new DependencyGraph();
            graph.RemoveDependency("a", "b");
            Assert.AreEqual(0, graph.Size);
        }

        /// <summary>
        /// Tests adding a new dependency to a newly created (empty) graph.
        /// </summary>
        [TestMethod]
        public void TestAddToEmptyGraph()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            Assert.AreEqual(1, graph.Size);
        }

        /// <summary>
        /// Tests replacing a non-existent node's dependents with an empty set of dependents.
        /// </summary>
        [TestMethod]
        public void TestReplaceMissingNodeWithEmptyDependents()
        {
            var graph = new DependencyGraph();
            graph.ReplaceDependents("a", new HashSet<string>());
            Assert.AreEqual(0, graph.Size);
        }

        /// <summary>
        /// Tests replacing a non-existent node's dependees with an empty set of dependees.
        /// </summary>
        [TestMethod]
        public void TestReplaceMissingNodeWithEmptyDependees()
        {
            var graph = new DependencyGraph();
            graph.ReplaceDependees("a", new HashSet<string>());
            Assert.AreEqual(0, graph.Size);
        }

        /// <summary>
        /// Tests that a graph which has dependencies reports a correct size.
        /// </summary>
        [TestMethod]
        public void TestGraphHasCorrectSize()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            Assert.AreEqual(2, graph.Size);
        }

        /// <summary>
        /// Tests that adding a duplicate dependency will not alter the graph.
        /// </summary>
        [TestMethod]
        public void TestAddDuplicateDependency()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "b");
            Assert.AreEqual(1, graph.Size);
        }

        /// <summary>
        /// Tests that HasDependees reports correctly for any given node.
        /// </summary>
        [TestMethod]
        public void TestHasDependees()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            Assert.IsFalse(graph.HasDependees("a"));
            Assert.IsTrue(graph.HasDependees("b"));
            Assert.IsTrue(graph.HasDependees("c"));
            Assert.IsFalse(graph.HasDependees("d"));
        }
        
        /// <summary>
        /// Tests that HasDependents reports correctly for any given node.
        /// </summary>
        [TestMethod]
        public void TestHasDependents()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            Assert.IsTrue(graph.HasDependents("a"));
            Assert.IsFalse(graph.HasDependents("b"));
            Assert.IsFalse(graph.HasDependents("c"));
            Assert.IsTrue(graph.HasDependents("d"));
        }

        /// <summary>
        ///Nonempty graph should contain something
        ///</summary>
        [TestMethod]
        public void ComplexGraphCount()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            var aDents = new HashSet<String>(graph.GetDependents("a"));
            var bDents = new HashSet<String>(graph.GetDependents("b"));
            var cDents = new HashSet<String>(graph.GetDependents("c"));
            var dDents = new HashSet<String>(graph.GetDependents("d"));
            var eDents = new HashSet<String>(graph.GetDependents("e"));
            var aDees = new HashSet<String>(graph.GetDependees("a"));
            var bDees = new HashSet<String>(graph.GetDependees("b"));
            var cDees = new HashSet<String>(graph.GetDependees("c"));
            var dDees = new HashSet<String>(graph.GetDependees("d"));
            var eDees = new HashSet<String>(graph.GetDependees("e"));
            Assert.IsTrue(aDents.Count == 2 & aDents.Contains("b") & aDents.Contains("c"));
            Assert.IsTrue(bDents.Count == 0);
            Assert.IsTrue(cDents.Count == 0);
            Assert.IsTrue(dDents.Count == 1 && dDents.Contains("c"));
            Assert.IsTrue(eDents.Count == 0);
            Assert.IsTrue(aDees.Count == 0);
            Assert.IsTrue(bDees.Count == 1 && bDees.Contains("a"));
            Assert.IsTrue(cDees.Count == 2 && cDees.Contains("a") && cDees.Contains("d"));
            Assert.IsTrue(dDees.Count == 0);
            Assert.IsTrue(dDees.Count == 0);
        }

        /// <summary>
        ///Nonempty graph should contain something
        ///</summary>
        [TestMethod]
        public void ComplexGraphIndexer()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            Assert.AreEqual(0, graph["a"]);
            Assert.AreEqual(1, graph["b"]);
            Assert.AreEqual(2, graph["c"]);
            Assert.AreEqual(0, graph["d"]);
            Assert.AreEqual(0, graph["e"]);
        }

        /// <summary>
        ///Removing from a DG 
        ///</summary>
        [TestMethod]
        public void Remove()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            graph.RemoveDependency("a", "b");
            Assert.AreEqual(2, graph.Size);
        }

        /// <summary>
        ///Replace on a DG
        ///</summary>
        [TestMethod]
        public void ReplaceDependents()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            graph.ReplaceDependents("a", new HashSet<string> {"x", "y", "z"});
            var aPends = new HashSet<string>(graph.GetDependents("a"));
            Assert.IsTrue(aPends.SetEquals(new HashSet<string> {"x", "y", "z"}));
        }

        /// <summary>
        ///Replace on a DG
        ///</summary>
        [TestMethod]
        public void ReplaceDependees()
        {
            var graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("d", "c");
            graph.ReplaceDependees("c", new HashSet<string> {"x", "y", "z"});
            var cDees = new HashSet<string>(graph.GetDependees("c"));
            Assert.IsTrue(cDees.SetEquals(new HashSet<string> {"x", "y", "z"}));
        }

        /// <summary>
        /// Tests replacing dependees for a node which does not already exist in the graph.
        /// </summary>
        [TestMethod]
        public void TestEmptyReplaceDependees()
        {
            DependencyGraph dg = new DependencyGraph();

            dg.ReplaceDependees("b", new HashSet<string> {"a"});

            Assert.AreEqual(1, dg.Size);
            Assert.IsTrue(new HashSet<string> {"b"}.SetEquals(dg.GetDependents("a")));
        }

        // ************************** STRESS TESTS ******************************** //
        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod]
        public void StressTest1()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char) ('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        // ********************************** ANOTHER STESS TEST ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod]
        public void StressTest8()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char) ('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependents
            for (int i = 0; i < SIZE; i += 4)
            {
                HashSet<string> newDents = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 7)
                {
                    newDents.Add(letters[j]);
                }
                t.ReplaceDependents(letters[i], newDents);

                foreach (string s in dents[i])
                {
                    dees[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDents)
                {
                    dees[s[0] - 'a'].Add(letters[i]);
                }

                dents[i] = newDents;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

        // ********************************** A THIRD STESS TEST ******************** //
        /// <summary>
        ///Using lots of data with replacement
        ///</summary>
        [TestMethod]
        public void StressTest15()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 100;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char) ('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 2; j < SIZE; j += 2)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Replace a bunch of dependees
            for (int i = 0; i < SIZE; i += 4)
            {
                HashSet<string> newDees = new HashSet<String>();
                for (int j = 0; j < SIZE; j += 7)
                {
                    newDees.Add(letters[j]);
                }
                t.ReplaceDependees(letters[i], newDees);

                foreach (string s in dees[i])
                {
                    dents[s[0] - 'a'].Remove(letters[i]);
                }

                foreach (string s in newDees)
                {
                    dents[s[0] - 'a'].Add(letters[i]);
                }

                dees[i] = newDees;
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }
    }
}