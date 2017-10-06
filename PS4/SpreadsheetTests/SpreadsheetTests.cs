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
            spreadsheet.SetContentsOfCell("a1", "1");
            spreadsheet.SetContentsOfCell("b5", "2");
            spreadsheet.SetContentsOfCell("f4", "3");
            Assert.AreEqual(1d, spreadsheet.GetCellContents("a1"), "Could not set a1");
            Assert.AreEqual(2d, spreadsheet.GetCellContents("b5"), "Could not set b5");
            Assert.AreEqual(3d, spreadsheet.GetCellContents("f4"), "Could not set f4");

            // Set Uppercase
            spreadsheet.SetContentsOfCell("A1", "4");
            spreadsheet.SetContentsOfCell("B5", "5");
            spreadsheet.SetContentsOfCell("F4", "6");
            Assert.AreEqual(4d, spreadsheet.GetCellContents("A1"), "Could not set A1");
            Assert.AreEqual(5d, spreadsheet.GetCellContents("B5"), "Could not set B5");
            Assert.AreEqual(6d, spreadsheet.GetCellContents("F4"), "Could not set F4");

            // Check for collisions
            Assert.AreEqual(1d, spreadsheet.GetCellContents("a1"), "a1 was changed!");
            Assert.AreEqual(2d, spreadsheet.GetCellContents("b5"), "b5 was changed!");
            Assert.AreEqual(3d, spreadsheet.GetCellContents("f4"), "f4 was changed!");
        }

        /// <summary>
        /// Tests setting and getting cell contents when those contents are doubles.
        /// </summary>
        [TestMethod]
        public void TestSetAndGetCellContentsWithDouble()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Set once
            spreadsheet.SetContentsOfCell("a1", "10");
            Assert.AreEqual(10d, spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetContentsOfCell("a1", "5");
            Assert.AreEqual(5d, spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetContentsOfCell("a2", "15");
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
            spreadsheet.SetContentsOfCell("a1", "cat");
            Assert.AreEqual("cat", spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetContentsOfCell("a1", "dog");
            Assert.AreEqual("dog", spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetContentsOfCell("a2", "pig");
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
            spreadsheet.SetContentsOfCell("a1", "=1 + 5");
            Assert.AreEqual(new Formula("1 + 5"), spreadsheet.GetCellContents("a1"), "Could not set a1");

            // Replace
            spreadsheet.SetContentsOfCell("a1", "=5 * (4 + 5)");
            Assert.AreEqual(new Formula("5 * (4 + 5)"), spreadsheet.GetCellContents("a1"), "Could not replace a1");

            // Set another cell
            spreadsheet.SetContentsOfCell("a2", "=11 - B4 + (6 * C3)");
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
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("AB!", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("A B", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("", "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("6a", "0"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("(d)", "0"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell("_____-___", "0"));
            Assert.ThrowsException<InvalidNameException>(
                () => spreadsheet.SetContentsOfCell("d-5", "=1 + 2"));

            // Null name
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell(null, "test"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.SetContentsOfCell(null, "0"));
            Assert.ThrowsException<InvalidNameException>(() =>
                spreadsheet.SetContentsOfCell(null, "=1 + 2"));
        }

        /// <summary>
        /// Tests setting cells to null contents.
        /// </summary>
        [TestMethod]
        public void TestSetNullContents()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.ThrowsException<ArgumentNullException>(() => spreadsheet.SetContentsOfCell("a1", null));
        }

        /// <summary>
        /// Tests that retrieving an empty cell will return an empty string.
        /// </summary>
        [TestMethod]
        public void TestGetEmptyCell()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.AreEqual("", spreadsheet.GetCellContents("a1"));
            Assert.AreEqual("", spreadsheet.GetCellContents("b5"));
            Assert.AreEqual("", spreadsheet.GetCellContents("J9"));
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
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("a1a"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("_d5"));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellContents("john_doe_5"));
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
            spreadsheet.SetContentsOfCell("a1", "1");
            spreadsheet.SetContentsOfCell("b5", "2");
            spreadsheet.SetContentsOfCell("f4", "3");

            // Check for the added cells
            CollectionAssert.AreEquivalent(new[] {"a1", "b5", "f4"},
                spreadsheet.GetNamesOfAllNonemptyCells().ToArray());

            // Clear a cell
            spreadsheet.SetContentsOfCell("b5", "");

            // Check that the cell is no longer returned
            CollectionAssert.AreEquivalent(new[] {"a1", "f4"}, spreadsheet.GetNamesOfAllNonemptyCells().ToArray());
        }

        /// <summary>
        /// Tests getting a cell's direct dependents.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // The cell to check.
            spreadsheet.SetContentsOfCell("a1", "10");

            // Those which depend on a1.
            spreadsheet.SetContentsOfCell("b1", "=a1 + c2");
            spreadsheet.SetContentsOfCell("d1", "=10 + (5 * a1)");

            // Those which do not depend on a1
            spreadsheet.SetContentsOfCell("c2", "=5 + 10");
            spreadsheet.SetContentsOfCell("a9", "=b1 + d1");

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
            CollectionAssert.AreEqual(new[] {"a1"}, spreadsheet.SetContentsOfCell("a1", "10").ToArray(),
                "Dependencies were returned when none should have been found.");

            // Direct dependency
            spreadsheet.SetContentsOfCell("a2", "=a1 + 5");
            CollectionAssert.AreEqual(new[] {"a1", "a2"}, spreadsheet.SetContentsOfCell("a1", "5").ToArray(),
                "The direct dependencies returned did not match.");

            // Indirect dependency
            spreadsheet.SetContentsOfCell("a3", "=a2 + 5");
            CollectionAssert.AreEquivalent(new[] {"a1", "a2", "a3"}, spreadsheet.SetContentsOfCell("a1", "5").ToArray(),
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
            spreadsheet.SetContentsOfCell("a1", "doggo");
            CollectionAssert.AreEquivalent(new[] {"a1"}, spreadsheet.GetNamesOfAllNonemptyCells().ToArray());
            Assert.AreEqual("doggo", spreadsheet.GetCellContents("a1"));

            // Remove cell
            spreadsheet.SetContentsOfCell("a1", "");
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
            spreadsheet.SetContentsOfCell("a1", "=a3 * d3");
            spreadsheet.SetContentsOfCell("a3", "=a2 - 5");

            // Adding this cell will cause the circular dependency.
            Assert.ThrowsException<CircularException>(() =>
                spreadsheet.SetContentsOfCell("a2", "=a1 + d5"));

            // Make sure nothing was changed
            Assert.AreEqual("", spreadsheet.GetCellContents("a2"));
            CollectionAssert.AreEquivalent(new[] {"d5"}, spreadsheet.SetContentsOfCell("d5", "10").ToArray());
        }

        /// <summary>
        /// Tests that formulas can be replaced and their dependencies (direct or indirect) are updated correctly.
        /// </summary>
        [TestMethod]
        public void TestReplaceFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // a2 depends on a1
            spreadsheet.SetContentsOfCell("a1", "=a2 + 0");
            CollectionAssert.AreEquivalent(new[] {"a1", "a2"}, spreadsheet.SetContentsOfCell("a2", "10").ToArray());

            // Replace a1 with double; a1 should no longer depend on a2
            spreadsheet.SetContentsOfCell("a1", "5");
            CollectionAssert.AreEquivalent(new[] {"a2"}, spreadsheet.SetContentsOfCell("a2", "5").ToArray(),
                "Direct dependency was not removed.");

            // Create indirect dependency: a3 depends on a1 via a2.
            spreadsheet.SetContentsOfCell("a1", "=a2 + 0");
            CollectionAssert.AreEquivalent(new[] {"a1", "a2"},
                spreadsheet.SetContentsOfCell("a2", "=a3 + 0").ToArray());
            CollectionAssert.AreEquivalent(new[] {"a1", "a2", "a3"},
                spreadsheet.SetContentsOfCell("a3", "10").ToArray());

            // Replace a2 with double; a1 should no longer depend on a3.
            spreadsheet.SetContentsOfCell("a2", "10");
            CollectionAssert.AreEquivalent(new[] {"a3"}, spreadsheet.SetContentsOfCell("a3", "cat").ToArray(),
                "Indirect dependency was not removed.");
        }

        /// <summary>
        /// Tests when a cell in a formula is empty, then when that empty cell is updated to a double.
        /// </summary>
        [TestMethod]
        public void TestEmptyCellInFormulaIsUpdated()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // A2 is not set
            spreadsheet.SetContentsOfCell("A1", "=A2 + 5");
            Assert.IsInstanceOfType(spreadsheet.GetCellValue("A1"), typeof(FormulaError));

            // A2 is set
            spreadsheet.SetContentsOfCell("A2", "10");
            Assert.AreEqual(15d, spreadsheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests that spreadsheets are marked as changed and unchanged when necessary.
        /// </summary>
        [TestMethod]
        public void TestChanged()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.IsFalse(spreadsheet.Changed);

            // Get contents
            spreadsheet.GetCellContents("A1");
            Assert.IsFalse(spreadsheet.Changed);

            // Get value
            spreadsheet.GetCellValue("A1");
            Assert.IsFalse(spreadsheet.Changed);

            // Get non-empty
            spreadsheet.GetNamesOfAllNonemptyCells();
            Assert.IsFalse(spreadsheet.Changed);

            // Save with no changes
            spreadsheet.Save(Guid.NewGuid().ToString("N") + ".xml");
            Assert.IsFalse(spreadsheet.Changed);

            // Set cell
            spreadsheet.SetContentsOfCell("A1", "dog");
            Assert.IsTrue(spreadsheet.Changed);

            // Save with changes
            spreadsheet.Save(Guid.NewGuid().ToString("N") + ".xml");
            Assert.IsFalse(spreadsheet.Changed);
        }

        /// <summary>
        /// Tests the null and invalid checks in the protected helper methods for 
        /// SetContentsOfCell that we voted not to change.
        /// </summary>
        [TestMethod]
        public void TestNullAndInvalidChecksInHelperSetters()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            var pSpreadsheet = new PrivateObject(spreadsheet);

            // Null names
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", null, 10d)));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", null, "test")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", null, new Formula("1+1"))));

            // Invalid Names
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", "A_1", 10d)));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", "A_1", "test")));
            Assert.ThrowsException<InvalidNameException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", "A_1", new Formula("1+1"))));

            // Null contents
            Assert.ThrowsException<ArgumentNullException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", new[] {typeof(string), typeof(string)},
                        new object[] {"A1", null})));
            Assert.ThrowsException<ArgumentNullException>(() =>
                ThrowPrivateObjectBaseException(() =>
                    pSpreadsheet.Invoke("SetCellContents", new[] {typeof(string), typeof(Formula)},
                        new object[] {"A1", null})));
        }

        /// <summary>
        /// Tests the GetCellValue method when using an invalid or null name.
        /// </summary>
        [TestMethod]
        public void TestGetCellValueWithInvalidName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue(null));
            Assert.ThrowsException<InvalidNameException>(() => spreadsheet.GetCellValue("A_1"));
        }

        /// <summary>
        /// A stress test in which 500 cells are created that depend on each other, then the last cell that all previous depend on is changed.
        /// On my computer, the adding part takes 13 seconds and the changing value takes no noticable extra time.
        /// </summary>
        [TestMethod]
        public void TestStressChangeValue()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Create 500 cells that depend on each other.
            for (var i = 0; i < 500; i++)
            {
                spreadsheet.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }

            // Change value of A499
            spreadsheet.SetContentsOfCell("A499", "5");
        }

        /// <summary>
        /// A stress test in which 500 cells are created that depend on each other, then the last cell that all previous depend on is changed.
        /// On my computer, the adding part takes 13 seconds and getting the value 10,000 times takes no noticable extra time.
        /// </summary>
        [TestMethod]
        public void TestStressGetValue()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            // Create 500 cells that depend on each other.
            for (var i = 0; i < 500; i++)
            {
                spreadsheet.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }

            // Get value of A1
            for (var i = 0; i < 10000; i++)
            {
                spreadsheet.GetCellValue("A1");
            }
        }
    }
}