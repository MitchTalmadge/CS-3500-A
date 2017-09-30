using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// Tests for the implementation of the AbstractSpreadsheet class authored by Mitch Talmadge.
    /// </summary>
    /// <author>Mitch Talmadge, u1031378</author>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Ensures that contents of cells can be set and retrieved without conflicts when using 
        /// varaible names which only differ in case.
        /// </summary>
        [TestMethod]
        public void TestSetAndGetCaseSensitivity()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Set Lowercase
            spreadsheet.SetCellContents("a1", 1d);
            spreadsheet.SetCellContents("cat", 2d);
            spreadsheet.SetCellContents("alt_f4", 3d);
            Assert.AreEqual(1d, spreadsheet.GetCellContents("a1"), "Could not set a1");
            Assert.AreEqual(2d, spreadsheet.GetCellContents("cat"), "Could not set cat");
            Assert.AreEqual(3d, spreadsheet.GetCellContents("alt_f4"), "Could not set alt_f4");

            // Set Uppercase
            spreadsheet.SetCellContents("A1", 4d);
            spreadsheet.SetCellContents("CAT", 5d);
            spreadsheet.SetCellContents("ALT_F4", 6d);
            Assert.AreEqual(4d, spreadsheet.GetCellContents("A1"), "Could not set A1");
            Assert.AreEqual(5d, spreadsheet.GetCellContents("CAT"), "Could not set CAT");
            Assert.AreEqual(6d, spreadsheet.GetCellContents("ALT_F4"), "Could not set ALT_F4");

            // Check for collisions
            Assert.AreEqual(1d, spreadsheet.GetCellContents("a1"), "a1 was changed!");
            Assert.AreEqual(2d, spreadsheet.GetCellContents("cat"), "cat was changed!");
            Assert.AreEqual(3d, spreadsheet.GetCellContents("alt_f4"), "alt_f4 was changed!");
        }

        /// <summary>
        /// Tests setting and getting cell contents when those contents are doubles.
        /// </summary>
        [TestMethod]
        public void TestSetAndGetCellContentsWithDouble()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Set once
            spreadsheet.SetCellContents("a1", 10d);
            Assert.AreEqual(10d, spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetCellContents("a1", 5d);
            Assert.AreEqual(5d, spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetCellContents("a2", 15d);
            Assert.AreEqual(15d, spreadsheet.GetCellContents("a2"), "Could not set a2");

            // Make sure it didn't collide with previous cell
            Assert.AreEqual(5d, spreadsheet.GetCellContents("a1"), "a1 changed after setting a2");
        }

        /// <summary>
        /// Tests setting and getting cell contents when those contents are strings.
        /// </summary>
        [TestMethod]
        public void TestSetAndGetCellContentsWithString()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Set once
            spreadsheet.SetCellContents("a1", "cat");
            Assert.AreEqual("cat", spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetCellContents("a1", "dog");
            Assert.AreEqual("dog", spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetCellContents("a2", "pig");
            Assert.AreEqual("pig", spreadsheet.GetCellContents("a2"), "Could not set a2");

            // Make sure it didn't collide with previous cell
            Assert.AreEqual("dog", spreadsheet.GetCellContents("a1"), "a1 changed after setting a2");
        }

        /// <summary>
        /// Tests setting and getting cell contents when those contents are Formulas.
        /// </summary>
        [TestMethod]
        public void TestSetAndGetCellContentsWithFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Set once
            spreadsheet.SetCellContents("a1", new Formula("1 + 5"));
            Assert.AreEqual(new Formula("1 + 5"), spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetCellContents("a1", new Formula("5 * (4 + 5)"));
            Assert.AreEqual(new Formula("5 * (4 + 5)"), spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetCellContents("a2", new Formula("11 - B4 + (6 * C3)"));
            Assert.AreEqual(new Formula("11 - B4 + (6 * C3)"), spreadsheet.GetCellContents("a2"), "Could not set a2");

            // Make sure it didn't collide with previous cell
            Assert.AreEqual(new Formula("5 * (4 + 5)"), spreadsheet.GetCellContents("a1"),
                "a1 changed after setting a2");
        }

        /// <summary>
        /// Tests setting cells with null or invalid names.
        /// </summary>
        [TestMethod]
        public void TestSetWithInvalidNames()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Invalid name format
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("AB!", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("A B", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("6a", 0d));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("(d)", 0d));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents("_____-___", 0d));
            Assert.ThrowsException<InvalidNameException>(
                () => spreadsheet.SetCellContents("d-5", new Formula("1 + 2")));

            // Null name
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents(null, "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetCellContents(null, 0d));
            Assert.ThrowsException<InvalidNameException>(() =>
                spreadsheet.SetCellContents(null, new Formula("1 + 2")));
        }

        /// <summary>
        /// Tests setting cells to null contents.
        /// </summary>
        [TestMethod]
        public void TestSetNullContents()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.ThrowsException<ArgumentNullException>(() => spreadsheet.SetCellContents("a1", (string) null));
            Assert.ThrowsException<ArgumentNullException>(() => spreadsheet.SetCellContents("a1", (Formula) null));
        }

        /// <summary>
        /// Tests that retrieving an empty cell will return an empty string.
        /// </summary>
        [TestMethod]
        public void TestGetEmptyCell()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.AreEqual("", spreadsheet.GetCellContents("a1"));
            Assert.AreEqual("", spreadsheet.GetCellContents("nathan_milot"));
            Assert.AreEqual("", spreadsheet.GetCellContents("hannah_potter"));
        }

        /// <summary>
        /// Tests the behavior of trying to retrieve a null or invalid cell.
        /// </summary>
        [TestMethod]
        public void TestGetInvalidCellName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents(null));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents(""));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("5A"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("a!1"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("_a1-2"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("()"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("(5 + 6)"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents(" a1 "));
        }

        /// <summary>
        /// Tests that all the non-empty cells' names can be retrieved.
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonEmptyCells()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Should be empty at first
            CollectionAssert.AreEqual(new string[0], spreadsheet.GetNamesOfAllNonemptyCells().ToArray());

            // Add some cells
            spreadsheet.SetCellContents("a1", 1d);
            spreadsheet.SetCellContents("cat", 2d);
            spreadsheet.SetCellContents("alt_f4", 3d);

            // Check for the added cells
            CollectionAssert.AreEquivalent(new[] {"a1", "cat", "alt_f4"},
                spreadsheet.GetNamesOfAllNonemptyCells().ToArray());

            // Clear a cell
            spreadsheet.SetCellContents("cat", "");

            // Check that the cell is no longer returned
            CollectionAssert.AreEquivalent(new[] {"a1", "alt_f4"}, spreadsheet.GetNamesOfAllNonemptyCells().ToArray());
        }

        /// <summary>
        /// Tests getting a cell's direct dependents.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // The cell to check.
            spreadsheet.SetCellContents("a1", 10d);

            // Those which depend on a1.
            spreadsheet.SetCellContents("b1", new Formula("a1 + c2"));
            spreadsheet.SetCellContents("d1", new Formula("10 + (5 * a1)"));

            // Those which do not depend on a1
            spreadsheet.SetCellContents("c2", new Formula("5 + 10"));
            spreadsheet.SetCellContents("daniel_kopta", new Formula("b1 + d1"));

            // Check direct dependents. (Called using a PrivateObject)
            var dependents = (IEnumerable<string>) new PrivateObject(spreadsheet).Invoke("GetDirectDependents", "a1");

            CollectionAssert.AreEquivalent(new[] {"b1", "d1"}, dependents.ToArray());
        }

        /// <summary>
        /// Tests getting a cell's direct dependents when the cell name is invalid.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependentsWithInvalidCellName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            var privateObject = new PrivateObject(spreadsheet);

            // The try catch is needed because private objects will throw TargetInvocationExceptions which contain the original exception.
            Assert.ThrowsException<ArgumentNullException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    privateObject.Invoke("GetDirectDependents", new object[] {null})));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "5A")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "a!1")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "_a1-2")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "()")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", "(5 + 6)")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() => privateObject.Invoke("GetDirectDependents", " a1 ")));
        }

        /// <summary>
        /// Convenience method for throwing the base exception of a TargetInvocationException that takes place
        /// when an exception occurs during a PrivateObject invokation.
        /// 
        /// Useful for Assert.ThrowsException to catch the most specific exception thrown.
        /// </summary>
        /// <param name="action"></param>
        private static void ThrowPrivateObjectBaseException(Func<object> action)
        {
            try
            {
                action();
            }
            catch (TargetInvocationException e)
            {
                throw e.GetBaseException();
            }
        }

        /// <summary>
        /// Tests that a correct set of dependencies are returned when setting a cell.
        /// </summary>
        [TestMethod]
        public void TestSetReturnsDependencies()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // One cell, no dependencies
            CollectionAssert.AreEqual(new[] {"a1"}, spreadsheet.SetCellContents("a1", 10d).ToArray(),
                "Dependencies were returned when none should have been found.");

            // Direct dependency
            spreadsheet.SetCellContents("a2", new Formula("a1 + 5"));
            CollectionAssert.AreEqual(new[] {"a1", "a2"}, spreadsheet.SetCellContents("a1", 5d).ToArray(),
                "The direct dependencies returned did not match.");

            // Indirect dependency
            spreadsheet.SetCellContents("a3", new Formula("a2 + 5"));
            CollectionAssert.AreEquivalent(new[] {"a1", "a2", "a3"}, spreadsheet.SetCellContents("a1", 5d).ToArray(),
                "The direct and indirect dependencies returned did not match.");
        }

        /// <summary>
        /// Tests the ability to "remove" the contents of a cell by setting it to an empty string.
        /// </summary>
        [TestMethod]
        public void TestRemoveCells()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Add cell
            spreadsheet.SetCellContents("a1", "doggo");
            CollectionAssert.AreEquivalent(new[] {"a1"}, spreadsheet.GetNamesOfAllNonemptyCells().ToArray());
            Assert.AreEqual("doggo", spreadsheet.GetCellContents("a1"));

            // Remove cell
            spreadsheet.SetCellContents("a1", "");
            CollectionAssert.AreEquivalent(new string[0], spreadsheet.GetNamesOfAllNonemptyCells().ToArray());
            Assert.AreEqual("", spreadsheet.GetCellContents("a1"));
        }

        /// <summary>
        /// Tests creating a circular dependency.
        /// </summary>
        [TestMethod]
        public void TestCircularDependency()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("a1", new Formula("a3 * d3"));
            spreadsheet.SetCellContents("a3", new Formula("a2 - 5"));

            // Adding this cell will cause the circular dependency.
            Assert.ThrowsException<CircularException>(() => spreadsheet.SetCellContents("a2", new Formula("a1 + d5")));

            // Make sure nothing was changed
            Assert.AreEqual("", spreadsheet.GetCellContents("a2"));
            CollectionAssert.AreEquivalent(new[] {"d5"}, spreadsheet.SetCellContents("d5", 10d).ToArray());
        }

        /// <summary>
        /// Tests that formulas can be replaced and their dependencies (direct or indirect) are updated correctly.
        /// </summary>
        [TestMethod]
        public void TestReplaceFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // a2 depends on a1
            spreadsheet.SetCellContents("a1", new Formula("a2 + 0"));
            CollectionAssert.AreEquivalent(new[] {"a1", "a2"}, spreadsheet.SetCellContents("a2", 10d).ToArray());

            // Replace a1 with double; a1 should no longer depend on a2
            spreadsheet.SetCellContents("a1", 5d);
            CollectionAssert.AreEquivalent(new[] {"a2"}, spreadsheet.SetCellContents("a2", 5d).ToArray(),
                "Direct dependency was not removed.");

            // Create indirect dependency: a3 depends on a1 via a2.
            spreadsheet.SetCellContents("a1", new Formula("a2 + 0"));
            CollectionAssert.AreEquivalent(new[] {"a1", "a2"},
                spreadsheet.SetCellContents("a2", new Formula("a3 + 0")).ToArray());
            CollectionAssert.AreEquivalent(new[] {"a1", "a2", "a3"}, spreadsheet.SetCellContents("a3", 10d).ToArray());

            // Replace a2 with double; a1 should no longer depend on a3.
            spreadsheet.SetCellContents("a2", 10d);
            CollectionAssert.AreEquivalent(new[] {"a3"}, spreadsheet.SetCellContents("a3", "cat").ToArray(),
                "Indirect dependency was not removed.");
        }
    }
}