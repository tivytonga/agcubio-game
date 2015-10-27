using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

// Eric Longberg
// CS 3500, PS5
// October 15, 2015
namespace Tests
{
    [TestClass]
    public class TestSpreadsheet
    {
        /// <summary>
        /// Stress tests the spreadsheet, saving/reading several times with randomized elements.
        /// (Adapted from graded tests 47-50)
        /// </summary>
        [TestMethod]
        public void TestStressful()
        {
            AbstractSpreadsheet sheet1;
            AbstractSpreadsheet sheet2;
            for (int testNumber = 0; testNumber < 10; testNumber++)
            {
                sheet1 = new Spreadsheet();
                Random rand = new Random(testNumber);
                for (int i = 0; i < 10000; i++)
                {
                    try
                    {
                        switch (rand.Next(3))
                        {
                            case 0:
                                sheet1.SetContentsOfCell(randomName(rand), "2.718");
                                break;
                            case 1:
                                sheet1.SetContentsOfCell(randomName(rand), "hello");
                                break;
                            case 2:
                                sheet1.SetContentsOfCell(randomName(rand), randomFormula(rand));
                                break;
                        }
                    }
                    catch (CircularException)
                    {
                    }
                }
                sheet1.Save("../../TestXMLs/StressTest.xml");

                sheet2 = new Spreadsheet("../../TestXMLs/StressTest.xml", s => true, s => s, "default");

                var names1 = new HashSet<string>(sheet1.GetNamesOfAllNonemptyCells());
                var names2 = new HashSet<string>(sheet2.GetNamesOfAllNonemptyCells());

                Assert.AreEqual(names1.Count, names2.Count);

                foreach (string name in names1)
                {
                    Assert.AreEqual(sheet1.GetCellContents(name), sheet2.GetCellContents(name));
                    Assert.AreEqual(sheet1.GetCellValue(name), sheet2.GetCellValue(name));
                }
            }

        }

        /// <summary>
        /// Test get value returns correct error when name is null.
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestGetCellValue1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.718");
            sheet.GetCellValue(null);
        }

        /// <summary>
        /// Test get value returns correct error when name is invalid.
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void TestGetCellValue2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.718");
            sheet.GetCellValue("invalidName");
        }

        /// <summary>
        /// Test get value returns correct value (string).
        /// </summary>
        [TestMethod]
        public void TestGetCellValue3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "hi");
            Assert.AreEqual("hi", (string)sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Test get value returns correct value (double).
        /// </summary>
        [TestMethod]
        public void TestGetCellValue4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.718");
            Assert.AreEqual(2.718, (double)sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Test get value returns correct value (FormulaError).
        /// </summary>
        [TestMethod]
        public void TestGetCellValue5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=C9");
            Assert.IsTrue(sheet.GetCellValue("A1") is FormulaError);
        }

        /// <summary>
        /// Test get value returns correct value (valid formulas).
        /// </summary>
        [TestMethod]
        public void TestGetCellValue6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=C9*2");
            sheet.SetContentsOfCell("C9", "=10*D13");
            sheet.SetContentsOfCell("D13", "5");

            Assert.AreEqual(100.0, (double)sheet.GetCellValue("A1"));
        }


        /// <summary>
        /// Test constructor correctly uses Normalize, and Changed is updated appropriately.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            Assert.IsFalse(sheet.Changed);

            sheet.SetContentsOfCell("a3", "=d5*2");
            sheet.SetContentsOfCell("D5", "10");
            sheet.SetContentsOfCell("other1", "hello");

            Assert.IsTrue(sheet.Changed);

            HashSet<string> names = new HashSet<string> { "A3", "D5", "OTHER1" };
            Assert.IsTrue(names.SetEquals(sheet.GetNamesOfAllNonemptyCells()));

            Assert.AreEqual(20, (double)sheet.GetCellValue("a3"));
            Assert.AreEqual("hello", sheet.GetCellValue("other1"));
        }

        /// <summary>
        /// Test constructor correctly uses IsValid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestConstructor2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => (s=="a1"), s => s, "default");
            sheet.SetContentsOfCell("b1", "invalidName");
        }

        /// <summary>
        /// Test constructor correctly keeps version.
        /// </summary>
        [TestMethod]
        public void TestConstructor3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => (s == "a1"), s => s, "not default");
            Assert.AreEqual("not default", sheet.Version);
        }

        /// <summary>
        /// Test we can correctly read a saved file version.
        /// </summary>
        [TestMethod]
        public void TestGetSavedVersion()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("default", sheet.GetSavedVersion("../../TestXMLs/Correct.xml"));
            Assert.AreEqual("NewVersion2.0", sheet.GetSavedVersion("../../TestXMLs/Correct2.xml"));
        }

        /// <summary>
        /// Test constructor correctly stores and reads from file.
        /// </summary>
        [TestMethod]
        public void TestReadFile1()
        {
            AbstractSpreadsheet sheet1 = new Spreadsheet(s => true, s => s, "2.0");
            sheet1.SetContentsOfCell("A1", "3.5");
            sheet1.SetContentsOfCell("B2", "=A1*2");
            sheet1.Save("test.xml");

            AbstractSpreadsheet sheet2 = new Spreadsheet("test.xml", s => true, s => s, "2.0");
            
            HashSet<string> names = new HashSet<string>{"A1","B2"};
            Assert.IsTrue(names.SetEquals(sheet2.GetNamesOfAllNonemptyCells()));

            Assert.AreEqual(new Formula("A1*2"), (Formula)sheet2.GetCellContents("B2"));
            Assert.AreEqual(7.0, sheet2.GetCellValue("B2"));

            Assert.AreEqual(sheet1.Version, sheet2.Version);
        }

        /// <summary>
        /// Test constructor throws error on file with circular dependency.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/CircularDependency.xml",s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with blank contents.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/BlankContents.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with blank names.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/BlankName.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on empty file.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/EmptyFile.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with extraneous elements.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/ExtraneousElement.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with invalid XML.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/InvalidXML.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with "contents" tag missing.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile8()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/MissingContents.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with "name" tag missing.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile9()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/MissingName.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with "version" attribute missing.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile10()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/NoVersion.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on non-existent file.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/IDoNotExist.xml", s => true, s => s, "default");
        }

        /// <summary>
        /// Test constructor throws error on file with differing version.
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void TestReadFile12()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/Correct.xml", s => true, s => s, "different version");
        }

        /// <summary>
        /// Test constructor does not throw error on a correct file.
        /// </summary>
        [TestMethod]
        public void TestReadFile13()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("../../TestXMLs/Correct.xml", s => true, s => s, "default");
        }




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
            sheet.SetContentsOfCell("C3", "20");
            sheet.SetContentsOfCell("C3", "new");
            sheet.SetContentsOfCell("A3", "=" + new Formula("A1*2").ToString());
            sheet.SetContentsOfCell("A4", "=" + new Formula("A2+3*A3").ToString());
            sheet.SetContentsOfCell("A5", "=" + new Formula("A4+A1").ToString());


            HashSet<string> names = new HashSet<string> { "A1", "A2", "B2", "C3", "A3", "A4", "A5" };
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
            sheet.SetContentsOfCell("string1", "content");
            sheet.SetContentsOfCell("double1", "5.6");
            sheet.SetContentsOfCell("formula1", "=" + new Formula("3*3").ToString());

            Assert.IsTrue(sheet.GetCellContents("string1") is string);
            Assert.IsTrue(sheet.GetCellContents("double1") is double);
            Assert.IsTrue(sheet.GetCellContents("formula1") is Formula);

            Assert.AreEqual("content", (string)sheet.GetCellContents("string1"));
            Assert.AreEqual(5.6, (double)sheet.GetCellContents("double1"));
            Assert.AreEqual(new Formula("3*3"), (Formula)sheet.GetCellContents("formula1"));

            Assert.AreEqual("", (string)sheet.GetCellContents("DNE1"));
        }

        /// <summary>
        /// Test that GetNames... contains all nonempty cells in a graph.
        /// </summary>
        [TestMethod]
        public void TestGetNames1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            HashSet<string> set = new HashSet<string> { "a1", "b1", "c1" };
            sheet.SetContentsOfCell("a1", "3");
            sheet.SetContentsOfCell("b1", "hello");
            sheet.SetContentsOfCell("c1", "=" + new Formula("3+3").ToString());
            Assert.IsTrue(set.SetEquals(sheet.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Test that GetNames... does not contain the empty cells.
        /// </summary>
        [TestMethod]
        public void TestGetNames2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            HashSet<string> set = new HashSet<string> { "b1", "c1" };
            sheet.SetContentsOfCell("a1", "3");
            sheet.SetContentsOfCell("b1", "hello");
            sheet.SetContentsOfCell("c1", "=" + new Formula("3+3").ToString());

            sheet.SetContentsOfCell("a1", "");
            sheet.SetContentsOfCell("lmnop1", "");

            Assert.IsTrue(set.SetEquals(sheet.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Tests that formulas will correctly depend on appropriate cells and nothing more (basic).
        /// </summary>
        [TestMethod]
        public void TestSetContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("foo1", "=" + new Formula("Cauchy1+4").ToString());
            sheet.SetContentsOfCell("bar1", "content");

            HashSet<string> actual = new HashSet<string>(sheet.SetContentsOfCell("Cauchy1", "3.14"));

            HashSet<string> expected = new HashSet<string>();

            expected.Add("foo1");
            expected.Add("Cauchy1");

            Assert.IsTrue(expected.SetEquals(actual));
        }

        /// <summary>
        /// Tests that formulas will correctly depend on each other (advanced).
        /// </summary>
        [TestMethod]
        public void TestSetContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=" + new Formula("b1+c1+d1").ToString());
            sheet.SetContentsOfCell("b1", "=" + new Formula("c1+d1").ToString());
            sheet.SetContentsOfCell("c1", "=" + new Formula("d1").ToString());

            HashSet<string> actual = new HashSet<string>(sheet.SetContentsOfCell("d1", "1"));

            HashSet<string> expected = new HashSet<string> { "a1", "b1", "c1", "d1" };

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
            sheet.SetContentsOfCell("a1", "=" + new Formula("a1").ToString());
        }

        /// <summary>
        /// Tests that CircularDependency will indeed occur (multiple elements).
        /// </summary>
        [ExpectedException(typeof(CircularException))]
        [TestMethod]
        public void TestSetContents4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=" + new Formula("b1").ToString());
            sheet.SetContentsOfCell("b1", "=" + new Formula("c1").ToString());
            sheet.SetContentsOfCell("c1", "=" + new Formula("a1").ToString());
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





        //////////////////////////////////////////// Graded tests from PS4 (with updates to fit PS5) //////////////////////////////////

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod()]
        public void Test3()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        // Was ArgumentNullException, but now want a FormulaFormatException (PS5)
        public void Test11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", "="+(Formula)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "="+new Formula("2"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test13()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "="+new Formula("2"));
        }

        [TestMethod()]
        public void Test14()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=" + new Formula("3"));
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2"));
            s.SetContentsOfCell("A2", "=" + new Formula("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3"));
            s.SetContentsOfCell("A3", "=" + new Formula("A4+A5"));
            s.SetContentsOfCell("A5", "=" + new Formula("A6+A7"));
            s.SetContentsOfCell("A7", "=" + new Formula("A1+A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test17()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=" + new Formula("A2+A3"));
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=" + new Formula("A3*A1"));
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test18()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test21()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test22()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=" + new Formula("3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test23()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=" + new Formula("3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=" + new Formula("5"));
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void Test25()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=" + new Formula("5"));
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test26()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=" + new Formula("5")).SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void Test27()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=" + new Formula("A2+A4"));
            s.SetContentsOfCell("A4", "=" + new Formula("A2+A5"));
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3"));
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("A2+A3"));
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=" + new Formula("23"));
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=" + new Formula("B1+B2"));
            s.SetContentsOfCell("B1", "=" + new Formula("C1-C2"));
            s.SetContentsOfCell("B2", "=" + new Formula("C3*C4"));
            s.SetContentsOfCell("C1", "=" + new Formula("D1*D2"));
            s.SetContentsOfCell("C2", "=" + new Formula("D3*D4"));
            s.SetContentsOfCell("C3", "=" + new Formula("D5*D6"));
            s.SetContentsOfCell("C4", "=" + new Formula("D7*D8"));
            s.SetContentsOfCell("D1", "=" + new Formula("E1"));
            s.SetContentsOfCell("D2", "=" + new Formula("E1"));
            s.SetContentsOfCell("D3", "=" + new Formula("E1"));
            s.SetContentsOfCell("D4", "=" + new Formula("E1"));
            s.SetContentsOfCell("D5", "=" + new Formula("E1"));
            s.SetContentsOfCell("D6", "=" + new Formula("E1"));
            s.SetContentsOfCell("D7", "=" + new Formula("E1"));
            s.SetContentsOfCell("D8", "=" + new Formula("E1"));
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=" + new Formula("A" + (i + 1)))));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=" + new Formula("A" + (i + 1)));
            }
            try
            {
                s.SetContentsOfCell("A150", "=" + new Formula("A50"));
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=" + new Formula("A1" + (i + 1)));
            }
            HashSet<string> firstCells = new HashSet<string>();
            HashSet<string> lastCells = new HashSet<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.Add("A1" + i);
                lastCells.Add("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SetEquals(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SetEquals(lastCells));
        }
        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }

        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }




        /////////////////////////Graded tests from PS5
        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("q:\\missing\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod()]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod()]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join(60 * 1000);
            if (t.IsAlive)
            {
                t.Abort();
                Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 15;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

    }

}
