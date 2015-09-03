using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// Eric Longberg
/// CS 3500, PS1
/// September 10, 2015
namespace FormulaEvaluator
{
    /// <summary>
    /// Evaluator of standard infix notation.
    /// Supports the operators +, -, *, / and allows for parenthesis,
    /// non-negative integers, and variables consisting of 1+ letters
    /// followed by 1+ digits. Whitespace is ignored.
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string[] substrings = System.Text.RegularExpressions.Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return 1;
        }
    }
}
