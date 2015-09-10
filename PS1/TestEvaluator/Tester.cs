using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEvaluator
{
    class Tester
    {
        static void Main(string[] args)
        {
            List<string> expressions = new List<string>();

            expressions.Add("5*A2b/(0-X)");
            expressions.Add("5");
            expressions.Add("xb3b");
            expressions.Add("0/3");
            expressions.Add("/73+4");
            expressions.Add("5*7+5*7+5*7+5*7+5*7+5*7+5*7+5*7+5*7");
            expressions.Add("5*7+5*7+5*7+5*7+5*7+5*7+5*7+5*7+5*");
            expressions.Add("(7)+400*(2+(1/(X+A)))");
            expressions.Add("(((7+(3))-5))");
            expressions.Add("3*5-6/4+2");
            expressions.Add("4+3*8/4-2");
            expressions.Add("(2*(0-8)-(0-2))/(5*(0-2)+3)");
            expressions.Add("(2*x*x*x-x)/(y*x+y)");
            expressions.Add("");
            expressions.Add("5+[6]");

            foreach (string exp in expressions)
            {
                Console.Write(exp + " = ");
                try
                {
                    Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate(exp, variableEvaluator));
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Error");
                }
            }
            
            Console.Read();
        }

        private static int variableEvaluator(string v)
        {
            Dictionary<string, int> map = new Dictionary<string, int>();
            map.Add("A", 30);
            map.Add("X", 5);
            map.Add("A2", 10);
            map.Add("x", -2);
            map.Add("y", 3);

            return map[v];
        }
    }
}
