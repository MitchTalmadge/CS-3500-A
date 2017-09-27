using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
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
            Assert.AreEqual(new Formula("5 * (4 + 5)"), spreadsheet.GetCellContents("a1"), "a1 changed after setting a2");
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

            // Check direct dependents.
            var dependents = ((IEnumerable<string>) typeof(Spreadsheet).InvokeMember("GetDirectDependents",
                BindingFlags.InvokeMethod, null, spreadsheet, new[] {"a1"})).ToArray();

            Assert.AreEqual(2, dependents.Length, "There were too many direct dependents.");
            CollectionAssert.Contains(dependents, "b1", "The direct dependents does not include b1.");
            CollectionAssert.Contains(dependents, "d1", "The direct dependents does not include d1.");
            CollectionAssert.DoesNotContain(dependents, "c2", "The direct dependents includes c2.");
            CollectionAssert.DoesNotContain(dependents, "daniel_kopta", "The direct dependents includes daniel_kopta.");
        }
    }
}
