using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTester
    {

        //////// Tests on GetVariables ////////////
        
        /// <summary>
        /// Test GetVariables contains only the normalized variables.
        /// </summary>
        [TestMethod]
        public void publicTestGetVariables1()
        {
            List<string> norm = new List<string> {"A", "C4", "G"};
            Formula f = new Formula("a+5e4+G+c4", s => s.ToUpper(), s => true);
            foreach (string var in f.GetVariables())
            {
                Assert.IsTrue(norm.Contains(var));
            }
        }

        /// <summary>
        /// Test GetVariables contains only one of each variable.
        /// </summary>
        [TestMethod]
        public void publicTestGetVariables2()
        {
            HashSet<string> expected = new HashSet<string> { "A", "C", "G" };
            Formula f = new Formula("a+ 5e4+G+c/ (A*2-C*g)+3.0", s => s.ToUpper(), s => true);
            
            int count = 0;
            foreach (string var in f.GetVariables())
            {
                Assert.IsTrue(expected.Contains(var));
                count++;
            }
            Assert.AreEqual(expected.Count, count);
        }

        ///////// Tests on ToString ///////////
        
        /// <summary>
        /// Test ToString returns a normalized result without whitespace.
        /// </summary>
        [TestMethod]
        public void publicTestToString1()
        {
            Formula f1 = new Formula("x + y+z", s => s.ToUpper(), s => true);
            Assert.IsTrue(f1 == new Formula(f1.ToString()));
            Assert.AreEqual("X+Y+Z",f1.ToString());
        }

        /// <summary>
        /// Test ToString on a more complicated formula.
        /// </summary>
        [TestMethod]
        public void publicTestToString2()
        {
            Formula f1 = new Formula("x+ Y-3.140*(z3)/ 5e2");
            Assert.IsTrue(f1 == new Formula(f1.ToString()));
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
