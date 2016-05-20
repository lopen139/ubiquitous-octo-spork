using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public class SudokuSolver2
    {
        public Sudoku2 sudoku;
        public int xbest = 0;
        private bool solved;

        public long steps = 0;
        public long solveTime;
        public long solveTicks;

        public Sudoku2 SolveSudoku(string[] input)
        {
            //implementatie van opdracht 2
            sudoku = new Sudoku2(input);
            sudoku.PrintSudoku();
            sudoku.InitializePossGrid();
            Solve(new Operation(0, 0, 0));
            return sudoku;
        }
        public Sudoku2 SolveSudoku(int[,] input, int n)
        {
            sudoku = new Sudoku2(input, n);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            sudoku.InitializePossGrid();
            Solve(new Operation(0, 0, 0));
            watch.Stop();
            solveTime = watch.ElapsedMilliseconds;
            solveTicks = watch.ElapsedTicks;
            return sudoku;
        }

        private void Solve(Operation parentOperation)
        {
            steps++;
            bool moreChildren = true;
            var lastOperation = parentOperation;
            while (!solved && moreChildren)
            {
                if (xbest >= sudoku.n * sudoku.n)
                {
                    solved = true;
                }
                else
                {

                    if (sudoku.possGrid[xbest].Item1 == 0) xbest++;
                    else
                    {
                        var operation = sudoku.GetNewSudoku(lastOperation, xbest);
                        if (operation.val == -2 || xbest >= sudoku.n * sudoku.n)
                        {
                            solved = true;
                        }
                        else if (operation.val == -1)
                        {
                            moreChildren = false;
                            sudoku.UndoLastOperation(parentOperation);
                            xbest--;
                        }
                        else
                        {
                            sudoku.AugmentSudoku(operation);
                            /* if (xbest > 25)
                             {
                                 Console.Clear();
                                 sudoku.PrintSudoku();
                                 System.Threading.Thread.Sleep(110);
                             }*/
                            xbest++;
                            Solve(operation);
                            lastOperation = operation;
                        }
                    }
                }
            }
        }

    }
    public class Sudoku2
    {
        public int n;
        private int[,] puzzle;
        //public int[,] possGrid;
        public Tuple<int, int, int>[] possGrid;
        private int sqrtN;

        public Sudoku2(string[] input)
        {

            n = input.Length;
            puzzle = new int[n, n];
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

        public Sudoku2(int[,] _puzzle, int _n)
        {
            puzzle = _puzzle;
            n = _n;
            sqrtN = Convert.ToInt32(Math.Sqrt(n));
        }

        public Operation GetNewSudoku(Operation lastOperation, int xbest)
        {
            int val = 0;
            if (possGrid[xbest].Item2 == lastOperation.x && possGrid[xbest].Item3 == lastOperation.y) val = lastOperation.val; //still checking same square
            bool found = false;
            int x = possGrid[xbest].Item2, y = possGrid[xbest].Item3;
            while (!found)
            {
               /* if (xbest >= n*n)
                {
                    if (CheckSudoku()) return new Operation(0, 0, -2);
                    else return new Operation(0, 0, -1);                 
                }
                else
                { x = possGrid[xbest].Item2; y = possGrid[xbest].Item3; }*/

                /*if (possGrid[xbest].Item1 == 0) xbest++;
                else*/ if (val < n)
                {
                    val++;
                    found = TestOperation(x, y, val);
                }
                else
                {
                    return new Operation(0, 0, -1); // Branch is dead
                }
            }
            return new Operation(x, y, val);
        }

        public void PrintSudoku()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(puzzle[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        public void AugmentSudoku(Operation opp)
        {
            puzzle[opp.x, opp.y] = opp.val;
        }

        private bool TestOperation(int x, int y, int val)
        {
            if (puzzle[x, y] != 0) return false;
            //Test horizontal
            for (int i = 0; i < n - 1; i++)
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
        private int countPoss(int x, int y)
        {
            //square isn't empty
            if (puzzle[x, y] != 0) return 0;

            bool[] bools = new bool[n + 1];
            for (int i = 0; i < n + 1; i++)
            { bools[i] = false; }
            //check horizontal and vertical lines
            for (int i = 0; i < n; i++)
            {
                bools[puzzle[x, i]] = true;
                bools[puzzle[i, y]] = true;
            }
            //check surrouding square
            int x_block = x - (x % sqrtN);
            int y_block = y - (y % sqrtN);
            for (int i = x_block; i < x_block + sqrtN; i++)
            {
                for (int j = y_block; j < y_block + sqrtN; j++)
                {
                    bools[puzzle[i, j]] = true;
                }
            }
            int num = n;
            for (int i = 1; i < n + 1; i++)
            {
                if (bools[i]) num--;
            }
            return num;
        }
        public void InitializePossGrid()
        {
            possGrid = new Tuple<int, int, int>[n * n];
            int x = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //tpossGrid[i,j] = countPoss(i, j);
                    int val = countPoss(i, j);
                    possGrid[x] = new Tuple<int, int, int>(val, i, j);
                    x++;
                }
            }
            Array.Sort(possGrid);

        }
    }


}
