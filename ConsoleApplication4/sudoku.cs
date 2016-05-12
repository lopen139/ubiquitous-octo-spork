using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Sudoku
    {
        private int n;
        private int[,] puzzle;

        public Sudoku(string[] input)
        {
            n = input.Length;
            puzzle = new int[n,n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    string numstr = input[i][j].ToString();
                    puzzle[i, j] = int.Parse(numstr);
                }
            }
        }

        public void PrintSudoku()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(puzzle[i,j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
