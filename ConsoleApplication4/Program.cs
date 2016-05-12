using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = System.IO.File.ReadAllLines("..\\..\\puzzels\\1.txt");
            var sudoku = new Sudoku(input);
            sudoku.PrintSudoku();
            Console.ReadLine();
        }
    }
}
