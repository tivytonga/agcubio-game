using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTester
    {
        /// <summary>
        /// Test equality for two null formulas.
        /// </summary>
        [TestMethod]
        public void publicTestEquality1()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f1);
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }

        /// <summary>
        /// Test inequality with one null formula.
        /// </summary>
        [TestMethod]
        public void publicTestEquality2()
        {
            Formula f1 = null;
            Formula f2 = new Formula("x1");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f2 == f1);
            Assert.IsTrue(f1 != f2);
            Assert.IsTrue(f2 != f1);
        }

        /// <summary>
        /// Test equality with spaces.
        /// </summary>
        [TestMethod]
        public void publicTestEquality3()
        {
            Formula f1 = new Formula("X1+Y1");
            Formula f2 = new Formula("X1 + Y1");
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f1);
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }

        /// <summary>
        /// Test equality with doubles.
        /// </summary>
        [TestMethod]
        public void publicTestEquality4()
        {
            Formula f1 = new Formula("3.14");
            Formula f2 = new Formula("3.140000");
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f1);
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }

        /// <summary>
        /// Test inequality with associativity.
        /// </summary>
        [TestMethod]
        public void publicTestEquality5()
        {
            Formula f1 = new Formula("A4+2.0");
            Formula f2 = new Formula("2.0+A4");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f2 == f1);
            Assert.IsTrue(f1 != f2);
            Assert.IsTrue(f2 != f1);
        }

        /// <summary>
        /// Test equality with normalization.
        /// </summary>
        [TestMethod]
        public void publicTestEquality6()
        {
            Formula f1 = new Formula("x+y", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("X+Y");
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f1);
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }
    }
}
