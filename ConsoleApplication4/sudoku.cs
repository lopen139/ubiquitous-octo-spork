using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

    internal class SudokuSolver
    {
        public Sudoku sudoku;
        private bool solved;

        public Sudoku SolveSudoku(string[] input)
        {
            sudoku = new Sudoku(input);
            Solve(new Operation(0,0,0));
            return sudoku;
        }

        public void Solve(Operation lastOperation)
        {
            //TODO: iets met een stack en een while loop..
            //TODO: check voor "dead end": kijken of de sudoku helemaal is ingevuld of niet. Test sudoku methode?
            //TODO: alleen nullen morgen verandert worden.

            //Generate children
            bool moreChildren = true;
            var parentOperation = lastOperation;
            while (!solved && moreChildren)
            {
                var operation = sudoku.GetNewSudoku(lastOperation);
                if (operation.val == -2)
                {
                    solved = true;
                }
                else if (operation.val == -1)
                {
                    moreChildren = false;
                    //undo
                    sudoku.UndoLastOperation(parentOperation);
                }
                else
                {
                    sudoku.AugmentSudoku(operation);
                    //Console.Clear();
                    //sudoku.PrintSudoku();
                    //System.Threading.Thread.Sleep(300);
                    Solve(operation);
                    lastOperation = operation;
                }
            }
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
                if (val < n && puzzle[x, y] == 0)
                {
                    val++;
                    found = TestOperation(x, y, val);
                }
                else if (puzzle[x, y] == 0)
                {
                    return new Operation(0,0,-1); // Branch is dead
                }
                else if (y < n - 1 )
                {
                    y++;
                    //x = 0;
                    val = 0;
                    //found = TestOperation(x, y, val);
                }
                else if (x < n - 1)
                {
                    x++;
                    y = 0;
                    val = 0;
                   // found = TestOperation(x, y, val);
                }

                else
                {
                    return new Operation(0,0,-2); // Answer found
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
