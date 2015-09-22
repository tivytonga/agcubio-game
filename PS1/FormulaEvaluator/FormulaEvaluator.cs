using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Eric Longberg
// CS 3500, PS1
// September 10, 2015
namespace FormulaEvaluator
{
    /// <summary>
    /// Basic evaluator of standard infix notation.
    /// Supports the operators +, -, *, / and allows for parenthesis,
    /// non-negative integers, and variables consisting of 1+ letters
    /// followed by 1+ digits. Whitespace is ignored. Uses double arithmetic before
    /// truncating the final result.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Should take a given string (variable) and return an int value for it.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// Evaluates the given expression, using double arithmetic but returning an int.
        /// Returns ArgumentException if an error is detecting in the given expression.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = System.Text.RegularExpressions.Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            Stack<string> opStack = new Stack<string>();
            Stack<double> valStack = new Stack<double>();

            foreach (string s in substrings)
            {
                // Ignore the whitespace
                if (String.IsNullOrWhiteSpace(s))
                    continue;

                // Try to read an integer
                double num;
                if (Double.TryParse(s, out num))
                {
                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            if (valStack.Count < 1)
                                throw new ArgumentException();
                            opStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, num));
                        }
                        else
                            valStack.Push(num);
                    }
                    else
                        valStack.Push(num);
                    continue;
                }

                // Try to read + or -
                if (s.Equals("+") || s.Equals("-"))
                {
                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("+") || op.Equals("-"))
                        {
                            if (valStack.Count < 2)
                                throw new ArgumentException();

                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, rightValue));
                        }
                    }

                    opStack.Push(s);
                    continue;
                }

                // Try to read *, /, (
                if (s.Equals("*") || s.Equals("/") || s.Equals("("))
                {
                    opStack.Push(s);
                    continue;
                }

                // Try to read )
                if (s.Equals(")"))
                {
                    string op;
                    if (opStack.Count > 0)
                    {
                        op = opStack.Peek();
                        if (op.Equals("+") || op.Equals("-"))
                        {
                            if (valStack.Count < 2)
                                throw new ArgumentException();

                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, rightValue));
                        }
                    }

                    if (opStack.Count == 0)
                        throw new ArgumentException();
                    op = opStack.Pop();
                    if (!op.Equals("("))
                        throw new ArgumentException();

                    if (opStack.Count > 0)
                    {
                        op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            if (valStack.Count < 2)
                                throw new ArgumentException();

                            opStack.Pop();
                            double rightValue = valStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, rightValue));
                        }
                    }

                    continue;
                }

                // Try to read a variable
                else
                {
                    if (!isValidVariableName(s))
                        throw new ArgumentException("Invalid variable " + s);

                    num = variableEvaluator(s);

                    if (opStack.Count > 0)
                    {
                        string op = opStack.Peek();
                        if (op.Equals("*") || op.Equals("/"))
                        {
                            if (valStack.Count < 1)
                                throw new ArgumentException();
                            opStack.Pop();
                            double leftValue = valStack.Pop();
                            valStack.Push(applyOperation(op, leftValue, num));
                        }
                        else
                            valStack.Push(num);
                    }
                    else
                        valStack.Push(num);
                    continue;
                }
            }

            if (opStack.Count < 1)
            {
                if (valStack.Count != 1)
                {
                    throw new ArgumentException();
                }
                return (int)valStack.Pop();
            }
            else if (opStack.Count == 1 && valStack.Count == 2)
            {
                double valRight = valStack.Pop();
                double valLeft = valStack.Pop();
                string op = opStack.Pop();
                if (op.Equals("+") || op.Equals("-"))
                {
                    return (int)applyOperation(op, valLeft, valRight);
                }
                else
                    throw new ArgumentException();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Applies the give operation (+, -, *, /) to the given parameters.
        /// Throws ArgumentException if divides by 0 or invalid operation.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        private static double applyOperation(string op, double leftValue, double rightValue)
        {
            if (op.Equals("+"))
                return leftValue + rightValue;
            if (op.Equals("-"))
                return leftValue - rightValue;
            if (op.Equals("*"))
                return leftValue * rightValue;
            if (op.Equals("/"))
            {
                if (rightValue.Equals(0))
                    throw new ArgumentException("Division by 0.");
                return leftValue / rightValue;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Returns true if the given string consists only of one or more letters followed
        /// by one or more digits.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool isValidVariableName(string name)
        {
            int testIndex = 0;
            while (Char.IsLetter(name, testIndex))
            {
                testIndex++;
                if (testIndex == name.Length)
                    // Only letters
                    return false;
            }

            if (testIndex == 0)
                // Didn't start with a letter
                return false;

            while (Char.IsDigit(name, testIndex))
            {
                testIndex++;
                if (testIndex == name.Length)
                    break;
            }

            if (testIndex != name.Length)
                // Wasn't all digits after the letters
                return false;

            return true;
        }
    }
}
