using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.Diagnostics;

// Eric Longberg
// CS 3500, PS4
// October 1, 2015
namespace Tests
{
    [TestClass]
    public class TestSpreadsheet
    {
        // Todo: update existing tests to fit changes in PS5
        // Todo: add new tests

        /// <summary>
        /// Tests the spreadsheet's dependencies capabilities.
        /// </summary>
        [TestMethod]
        public void TestComplex()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5");
            sheet.SetContentsOfCell("A2", "=" + new Formula("5-A1").ToString());
            sheet.SetContentsOfCell("B2", "stringy");
            sheet.SetContentsOfCell("name", "20");
            sheet.SetContentsOfCell("name", "new");
            sheet.SetContentsOfCell("A3", "=" + new Formula("A1*2").ToString());
            sheet.SetContentsOfCell("A4", "=" + new Formula("A2+3*A3").ToString());
            sheet.SetContentsOfCell("A5", "=" + new Formula("A4+A1").ToString());


            HashSet<string> names = new HashSet<string> { "A1", "A2", "B2", "name", "A3", "A4", "A5" };
            Assert.IsTrue(names.SetEquals(sheet.GetNamesOfAllNonemptyCells()));

            HashSet<string> A1dependents = new HashSet<string> { "A1", "A2", "A3", "A4", "A5" };
            Assert.IsTrue(A1dependents.SetEquals(sheet.SetContentsOfCell("A1", "5")));

            HashSet<string> A4dependents = new HashSet<string> { "A4", "A5" };
            Assert.IsTrue(A4dependents.SetEquals(sheet.SetContentsOfCell("A4", "=" + new Formula("A2+3*A3").ToString())));
        }

        /// <summary>
        /// Test that GetCellContents gives correct exception (null name).
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestGetContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);
        }

        /// <summary>
        /// Test that GetCellContents gives correct exception (invalid name).
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestGetContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("&%$");
        }

        /// <summary>
        /// Test that GetCellContents correctly returns contents of cell.
        /// </summary>
        [TestMethod]
        public void TestGetContents3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("string", "content");
            sheet.SetContentsOfCell("double", "5.6");
            sheet.SetContentsOfCell("formula", "=" + new Formula("3*3").ToString());

            Assert.IsTrue(sheet.GetCellContents("string") is string);
            Assert.IsTrue(sheet.GetCellContents("double") is double);
            Assert.IsTrue(sheet.GetCellContents("formula") is Formula);

            Assert.AreEqual("content", (string)sheet.GetCellContents("string"));
            Assert.AreEqual(5.6, (double)sheet.GetCellContents("double"));
            Assert.AreEqual("=" + new Formula("3*3"), (Formula)sheet.GetCellContents("formula"));

            Assert.AreEqual("", (string)sheet.GetCellContents("DNE"));
        }

        /// <summary>
        /// Test that GetNames... contains all nonempty cells in a graph.
        /// </summary>
        [TestMethod]
        public void TestGetNames1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            HashSet<string> set = new HashSet<string> { "a", "b", "c" };
            sheet.SetContentsOfCell("a", "3");
            sheet.SetContentsOfCell("b", "hello");
            sheet.SetContentsOfCell("c", "=" + new Formula("3+3").ToString());
            Assert.IsTrue(set.SetEquals(sheet.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Test that GetNames... does not contain the empty cells.
        /// </summary>
        [TestMethod]
        public void TestGetNames2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            HashSet<string> set = new HashSet<string> { "b", "c" };
            sheet.SetContentsOfCell("a", "3");
            sheet.SetContentsOfCell("b", "hello");
            sheet.SetContentsOfCell("c", "=" + new Formula("3+3").ToString());

            sheet.SetContentsOfCell("a", "");
            sheet.SetContentsOfCell("lmnop", "");

            Assert.IsTrue(set.SetEquals(sheet.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Tests that formulas will correctly depend on appropriate cells and nothing more (basic).
        /// </summary>
        [TestMethod]
        public void TestSetContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("foo", "=" + new Formula("Cauchy+4").ToString());
            sheet.SetContentsOfCell("bar", "content");

            HashSet<string> actual = new HashSet<string>(sheet.SetContentsOfCell("Cauchy", "3.14"));

            HashSet<string> expected = new HashSet<string>();

            expected.Add("foo");
            expected.Add("Cauchy");

            Assert.IsTrue(expected.SetEquals(actual));
        }

        /// <summary>
        /// Tests that formulas will correctly depend on each other (advanced).
        /// </summary>
        [TestMethod]
        public void TestSetContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a", "=" + new Formula("b+c+d").ToString());
            sheet.SetContentsOfCell("b", "=" + new Formula("c+d").ToString());
            sheet.SetContentsOfCell("c", "=" + new Formula("d").ToString());

            HashSet<string> actual = new HashSet<string>(sheet.SetContentsOfCell("d", "1"));

            HashSet<string> expected = new HashSet<string> { "a", "b", "c", "d" };

            Assert.IsTrue(expected.SetEquals(actual));
        }

        /// <summary>
        /// Tests that CircularDependency will indeed occur (1 element).
        /// </summary>
        [ExpectedException(typeof(CircularException))]
        [TestMethod]
        public void TestSetContents3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a", "=" + new Formula("a").ToString());
        }

        /// <summary>
        /// Tests that CircularDependency will indeed occur (multiple elements).
        /// </summary>
        [ExpectedException(typeof(CircularException))]
        [TestMethod]
        public void TestSetContents4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a", "=" + new Formula("b").ToString());
            sheet.SetContentsOfCell("b", "=" + new Formula("c").ToString());
            sheet.SetContentsOfCell("c", "=" + new Formula("a").ToString());
        }

        /// <summary>
        /// Test that SetContentsOfCell gives correct exception (invalid name, double).
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetContents5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$%&", "3.3");
        }

        /// <summary>
        /// Test that SetContentsOfCell gives correct exception (invalid name, string).
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetContents6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$%&", "hi");
        }

        /// <summary>
        /// Test that SetContentsOfCell gives correct exception (invalid name, Formula).
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetContents7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$%&", "=" + new Formula("3+5").ToString());
        }

        /// <summary>
        /// Test null name gives exception. (double arg)
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetNull1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "1");
        }

        /// <summary>
        /// Test null name gives exception. (string arg)
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetNull2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "a");
        }

        /// <summary>
        /// Test null name gives exception. (Formula arg)
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestSetNull3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "=" + new Formula("2+2").ToString());
        }

        /// <summary>
        /// Test null content gives exception. (string arg)
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestSetNull4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell("name", s);
        }

        /// <summary>
        /// Test null content gives exception. (Formula arg)
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestSetNull5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            String f = null;
            sheet.SetContentsOfCell("name", f);
        }

        /// <summary>
        /// Test both args null gives correct exception. (string arg)
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestSetNull6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell(null, s);
        }

        /// <summary>
        /// Test both args null gives correct exception. (Formula arg)
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestSetNull7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            String f = null;
            sheet.SetContentsOfCell(null, f);
        }
    }
}
