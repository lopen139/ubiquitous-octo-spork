using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{

    struct Operation
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

    class SudokuSolver
    {
        private Sudoku sudoku;

        public Sudoku SolveSudoku(string[] input)
        {
            sudoku = new Sudoku(input);

            

            return sudoku;
        }

        public void Solve(Operation lastOperation)
        {
            //TODO: iets met een stack en een while loop..
            //TODO: check voor "dead end": kijken of de sudoku helemaal is ingevuld of niet. Test sudoku methode?
            //TODO: alleen nullen morgen verandert worden.
            //Generate child
            var operation = sudoku.GetNewSudoku(lastOperation);
            if (operation.val == 0) return;
            sudoku.AugmentSudoku(operation);
           
            Solve(operation);
        }
    }

    class Sudoku
    {
        private int n;
        private int[,] puzzle;
        private int sqrtN;

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

        public Operation GetNewSudoku(Operation lastOperation)
        {
            int x = lastOperation.x;
            int y = lastOperation.y;
            int val = lastOperation.val;
            bool found = false;
            while (!found)
            {
                if (val < n)
                {
                    val++;
                    found = TestOperation(x, y, val);
                }
                else if (x < n - 1)
                {
                    x++;
                    val = 1;
                    found = TestOperation(x, y, val);
                }
                else if(y < n - 1)
                {
                    y++;
                    x = 0;
                    val = 1;
                    found = TestOperation(x, y, val);
                }
                else
                {
                    return new Operation(0,0,0);
                }
            }
            return new Operation(x,y,val);
        }

        public void AugmentSudoku(Operation opp)
        {
            puzzle[opp.x, opp.y] = opp.val;
        }

        private bool TestOperation(int x, int y, int val)
        {
            //Test horizontal
            for (int i = 0; i < n -1; i++)
            {
                if (val == puzzle[i, y]) return false;
            }
            //Test vertical
            for (int i = 0; i < n - 1; i++)
            {
                if (val == puzzle[x, i]) return false;
            }
            //Test block
            int x_block = x - x % sqrtN;
            int y_block = y - y % sqrtN;
            for (int i = x_block; i < x_block + sqrtN; i++)
            {
                for (int j = y_block; j < y_block + sqrtN; j++)
                {
                    if (val == puzzle[i, j]) return false;
                }   
            }
            return true;
        }
    }
}
