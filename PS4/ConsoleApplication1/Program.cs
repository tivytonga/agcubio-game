using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=3.14");

            s.SetContentsOfCell("B1", "=A1");

            s.SetContentsOfCell("A1", "5");

            Console.WriteLine(s.GetCellValue("B1"));
            Console.Read();
        }
    }
}
