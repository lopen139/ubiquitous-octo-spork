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
            Console.WriteLine("Please choose solve version.");
            int version = Int32.Parse(Console.ReadLine());
            var input = System.IO.File.ReadAllLines("..\\..\\puzzels\\1.txt");
            //var sudoku = new Sudoku(input);
            //sudoku.PrintSudoku();
            if (version == 1)
            {
                SudokuSolver solver = new SudokuSolver();
                solver.SolveSudoku(input);
                solver.sudoku.PrintSudoku();
            }
            else if (version == 2)
            {
                SudokuSolver2 solver = new SudokuSolver2();
                solver.SolveSudoku(input);
                solver.sudoku.PrintSudoku();
            }
            else
            {
                throw new Exception("Unrecognized version number!");
            }
            Console.ReadLine();
        }
    }
}
