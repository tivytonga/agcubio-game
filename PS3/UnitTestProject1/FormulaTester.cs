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






        //// Graded tests

        // Simple tests that return FormulaErrors
        [TestMethod()]
        public void Test16()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test17()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        public void Test18()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test19()
        {
            Formula f = new Formula("+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test20()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test21()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test22()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test23()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test24()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test25()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test26()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test27()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test28()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod()]
        public void Test29()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod()]
        public void Test30()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod()]
        public void Test31()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod()]
        public void Test32()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod()]
        public void Test33()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test34()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test35()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test36()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod()]
        public void Test37()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod()]
        public void Test38()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void Test39()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod()]
        public void Test40()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod()]
        public void Test41()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void Test42()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod()]
        public void Test43()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod()]
        public void Test44()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod()]
        public void Test45()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod()]
        public void Test46()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test47()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test48()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test49()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void Test50()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod()]
        public void Test51a()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51b()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51c()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51d()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        [TestMethod()]
        public void Test51e()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("3");
            Assert.IsTrue(f1.ToString().IndexOf("2") >= 0);
            Assert.IsTrue(f2.ToString().IndexOf("3") >= 0);
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52a()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52b()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52c()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52d()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod()]
        public void Test52e()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }
    }
}
