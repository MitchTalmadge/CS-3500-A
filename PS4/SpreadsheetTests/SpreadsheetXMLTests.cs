using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// This class tests loading and saving spreadsheets from/to XML.
    /// </summary>
    [TestClass]
    public class SpreadsheetXmlTests
    {
        private readonly Func<string, bool> _isValid = s => true;
        private readonly Func<string, string> _normalize = s => s;

        private const string XmlDir = "spreadsheets/";

        /* ---------------------- LOADING ---------------------- */

        [TestMethod]
        public void TestLoadValidFile()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet(XmlDir + "valid.xml", _isValid, _normalize, "default");

            Assert.AreEqual(new Formula("10 + 5"), spreadsheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("A1 + 5"), spreadsheet.GetCellContents("A2"));
            Assert.AreEqual(new Formula("A1 + A2"), spreadsheet.GetCellContents("B9"));
            Assert.AreEqual(10d, spreadsheet.GetCellContents("AB10"));
            Assert.AreEqual("Hello World", spreadsheet.GetCellContents("AB6"));
        }


        [TestMethod]
        public void TestLoadStringInFormula()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "stringInFormula.xml", _isValid, _normalize, "default");

            Assert.IsInstanceOfType(spreadsheet.GetCellValue("A1"), typeof(FormulaError));
        }

        [TestMethod]
        public void TestLoadOverwritingCell()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "overwrite.xml", _isValid, _normalize, "default");

            Assert.AreEqual("overwritten", spreadsheet.GetCellValue("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadWrongVersion()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "valid.xml", _isValid, _normalize, "not-default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadCircularDependency()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet(XmlDir + "circular.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadSpreadsheetMissingVersion()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "missingVersion.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadInvalidFormula()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "invalidFormula.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadInvalidName()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "invalidName.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadMissingCellTag()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "missingCellTag.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadMissingSpreadsheetTag()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "missingSpreadsheetTag.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadMissingName()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "missingName.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestLoadMissingContents()
        {
            AbstractSpreadsheet spreadsheet =
                new Spreadsheet(XmlDir + "missingContents.xml", _isValid, _normalize, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestEmptyFile()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet(XmlDir + "empty.xml", _isValid, _normalize, "default");
        }

        /* ---------------------- SAVING ---------------------- */

        [TestMethod]
        public void TestSaveAndReload()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();

            spreadsheet.SetContentsOfCell("A1", "=10+5");
            spreadsheet.SetContentsOfCell("A2", "=A1 + 5");
            spreadsheet.SetContentsOfCell("B9", "=A1 + A2");
            spreadsheet.SetContentsOfCell("AB10", "10");
            spreadsheet.SetContentsOfCell("AB6", "Hello World");

            // Prevent test collisions by picking a random file name.
            var randomName = Guid.NewGuid().ToString("N") + ".xml";

            // Save the file.
            spreadsheet.Save(randomName);
            
            // Try loading the saved file.
            AbstractSpreadsheet loadSpreadsheet = new Spreadsheet(randomName, _isValid, _normalize, "default");

            Assert.AreEqual(new Formula("10 + 5"), loadSpreadsheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("A1 + 5"), loadSpreadsheet.GetCellContents("A2"));
            Assert.AreEqual(new Formula("A1 + A2"), loadSpreadsheet.GetCellContents("B9"));
            Assert.AreEqual(10d, loadSpreadsheet.GetCellContents("AB10"));
            Assert.AreEqual("Hello World", loadSpreadsheet.GetCellContents("AB6"));
        }
    }
}