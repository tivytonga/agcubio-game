using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> l = new List<string>();
            l.Add("1");
            l.Add("2");
            l.Add("3");
            
            IEnumerable<string> ie = l.AsReadOnly();
            List<string> newl = (List<string>)ie;
            newl.Add("d");

            foreach (string k in l)
            {
                Console.WriteLine(k);
            }
            Console.Read();


            string a = "a";
            string a2 = "a";
            Console.WriteLine(a == a2);
            Console.Read();

            string formula = "(x+x_y - 4.0000 * y1) - 03x_d3d __a \n 3+3 - 4x";
            foreach (string s in GetTokens(formula))
            {
                Console.WriteLine(s);
            }
            Console.Read();
        }

        /// <summary>
        /// ( ) + - * /
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        public static String foo(object obj)
        {
            return obj.GetType().ToString();
        }
    }
}
