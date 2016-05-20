using System;

namespace ConsoleApplication4
{

    public struct Operation
    {
        public int x;
        public int y;
        public int val;

        public Operation(int _x, int _y, int _val)
        {
            x = _x;
            y = _y;
            val = _val;
        }
    }
    
    /// <summary>
    /// Sudoku SuperClass containing all the basic methods for sudoku's
    /// </summary>
    public class Sudoku
    {
        public int n;
        protected int[,] puzzle;
        protected int sqrtN;
        
        public Sudoku(int[,] _puzzle, int _n)
        {
            puzzle = _puzzle;
            n = _n;
            sqrtN = Convert.ToInt32(Math.Sqrt(n));
        }

        public Sudoku(string[] input)
        {

            n = input.Length;
            puzzle = new int[n,n];
            for (int i = 0; i < n; i++)
            {
                var temp = input[i].Split(' ');
                for (int j = 0; j < n; j++)
                {
                    string numstr = temp[j].ToString();
                    puzzle[i, j] = int.Parse(numstr);
                }
            }
            sqrtN = Convert.ToInt32(Math.Sqrt(n));
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
       
        public void AugmentSudoku(Operation opp)
        {
            puzzle[opp.x, opp.y] = opp.val;
        }

        /// <summary>
        /// Checks if sudoku is completely filled
        /// </summary>
        public bool CheckSudoku()
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    if (puzzle[i, j] == 0) return false;
                }
            }
            return true;
        }

        public void UndoLastOperation(Operation opp)
        {
            puzzle[opp.x, opp.y] = 0;
        }
     }
}
