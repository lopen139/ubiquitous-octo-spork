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
            var input = System.IO.File.ReadAllLines("..\\..\\puzzels\\25x25_1.txt");
            //var sudoku = new Sudoku(input);
            //sudoku.PrintSudoku();
            SudokuSolver solver = new SudokuSolver();
            solver.SolveSudoku(input);
            solver.sudoku.PrintSudoku();
            Console.ReadLine();
        }
    }
}
