using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTester
    {
        ///////////////////////////////// Tests on constructor ////////////////////////

        /// <summary>
        /// Test throws error on non-normalizable variable.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct1()
        {
            Formula f = new Formula("3+x", norm_x_error, s => true);
        }

        /// <summary>
        /// Test throws error on empty string.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct2()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// Test throws error on isValid(normalize(var)) false.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct3()
        {
            Formula f = new Formula("3.0 + x", s => s, s => false);
        }
         
        /// <summary>
        /// Test throws error on invalid syntax.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct4()
        {
            Formula f = new Formula("+A");
        }

        /// <summary>
        /// Test throws error on invalid syntax.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct5()
        {
            Formula f = new Formula("3-5/");
        }

        /// <summary>
        /// Test throws error on invalid syntax.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct6()
        {
            Formula f = new Formula("2+C*7)-3");
        }

        /// <summary>
        /// Test throws error on invalid syntax.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct7()
        {
            Formula f = new Formula("(5)8");
        }

        /// <summary>
        /// Test throws error on invalid syntax.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void publicTestConstruct8()
        {
            Formula f = new Formula("((((hi))");
        }

        ///////////////////////////// Tests on Evaluate ///////////////////////

        /// <summary>
        /// Test basic usage.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate1()
        {
            Formula f = new Formula("x + Y-0");
            Assert.AreEqual(10d, f.Evaluate(s => 5));
        }

        /// <summary>
        /// Test division by 0 gives FormulaError.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate2()
        {
            Formula f = new Formula("25/(6-x)");
            FormulaError e = (FormulaError)f.Evaluate(s => 6);
        }

        /// <summary>
        /// Test undefined variable gives FormulaError.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate3()
        {
            Formula f = new Formula("25*a");
            FormulaError e = (FormulaError)f.Evaluate(s => { throw new ArgumentException(); });
        }

        /// <summary>
        /// Test on something more complex.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate4()
        {
            Formula f = new Formula("(_x4 - y*3)-4/2e2+ 5. *2.00000");
            Assert.AreEqual(5.98, f.Evaluate(s => 2));
        }

        ///////////////////////////// Tests on GetVariables //////////////////
        
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

        /////////////////////////// Tests on ToString //////////////////
        
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

        /////////////////////// Helpers for tests ///////////////////////

        /// <summary>
        /// Throws an exception for variable "x", otherwise returns s.ToUpper().
        /// </summary>
        public static string norm_x_error(string s)
        {
            if (s == "x")
                throw new ArgumentException();
            return s.ToUpper();
        }
    }
}
