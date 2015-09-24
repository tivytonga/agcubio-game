using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTester
    {
        ///////// Tests on ToString ///////////
        
        /// <summary>
        /// Test ToString returns a normalized result.
        /// </summary>
        [TestMethod]
        public void publicTestToString1()
        {
            Formula f1 = new Formula("x+y", s => s.ToUpper(), s => true);
            Assert.IsTrue(f1 == new Formula(f1.ToString()));
        }

        /// <summary>
        /// Test ToString
        /// </summary>
        [TestMethod]
        public void publicTestToString2()
        {
        }

        ///////////////////////// Equality tests using == != Equals and GetHashCode //////////////////////////

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
            Assert.IsFalse(f2.Equals(f1));
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
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f1));
            Assert.AreEqual(f1.GetHashCode(),f2.GetHashCode());
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
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f1));
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }

        /// <summary>
        /// Test inequality with associativity.
        /// </summary>
        [TestMethod]
        public void publicTestEquality5()
        {
            Formula f1 = new Formula("3 + 2.0");
            Formula f2 = new Formula("2.0 + 3");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f2 == f1);
            Assert.IsFalse(f1.Equals(f2));
            Assert.IsFalse(f2.Equals(f1));
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
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f1));
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }

        /// <summary>
        /// Test equality with many things.
        /// </summary>
        [TestMethod]
        public void publicTestEquality7()
        {
            Formula f1 = new Formula("x+y-3.14*(z3)/5", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("X+Y - 3.14000000 * (Z3) / 5");
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f1);
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f1));
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f1);
        }
    }
}
