using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    //TODO: Test removing cells
    //TODO: Test get null/invalid cells
    //TODO: Test get direct dependents with null/invalid name
    //TODO: Test circular dependency when setting Formula content
    //TODO: Test setting a cell which used to be Formula, make sure dependency is gone

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
        /// Tests that all the non-empty cells' names can be retrieved.
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonEmptyCells()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Should be empty at first
            CollectionAssert.AreEqual(new string[] { }, spreadsheet.GetNamesOfAllNonemptyCells().ToArray());

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
        /// Tests that a correct set of dependencies are returned when setting a cell.
        /// </summary>
        [TestMethod]
        public void TestSetReturnsDependencies()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // One cell, no dependencies
            CollectionAssert.AreEqual(new string[] { }, spreadsheet.SetCellContents("a1", 10d).ToArray(), "Dependencies were returned when none should have been found.");

            // Direct dependency
            spreadsheet.SetCellContents("a2", new Formula("a1 + 5"));
            CollectionAssert.AreEqual(new[] {"a2"}, spreadsheet.SetCellContents("a1", 5d).ToArray(), "The direct dependencies returned did not match.");

            // Indirect dependency
            spreadsheet.SetCellContents("a3", new Formula("a2 + 5"));
            CollectionAssert.AreEquivalent(new[] {"a2", "a3"}, spreadsheet.SetCellContents("a1", 5d).ToArray(), "The direct and indirect dependencies returned did not match.");
        }
    }
}