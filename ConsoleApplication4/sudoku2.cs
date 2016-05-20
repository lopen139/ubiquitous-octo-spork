using System;
using System.Diagnostics;

namespace ConsoleApplication4
{
    /// <summary>
    /// Solver class for assignment 2
    /// </summary>
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
                            xbest++;
                            Solve(operation);
                            lastOperation = operation;
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Sudoku class for assigment 2
    /// </summary>
    public class Sudoku2 : Sudoku
    {
        public Tuple<int, int, int>[] possGrid;

        public Sudoku2(string[] input) : base(input)
        {
        }

        public Sudoku2(int[,] _puzzle, int _n) : base(_puzzle, _n)
        {
        }

        /// <summary>
        /// Generates the next operation that can be applied.
        /// </summary>
        public Operation GetNewSudoku(Operation lastOperation, int xbest)
        {
            int val = 0;
            if (possGrid[xbest].Item2 == lastOperation.x && possGrid[xbest].Item3 == lastOperation.y) val = lastOperation.val; //still checking same square
            bool found = false;
            int x = possGrid[xbest].Item2, y = possGrid[xbest].Item3;
            while (!found)
            {
                if (val < n)
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

        /// <summary>
        /// Tests if a certain opperation does nog violate any sudoku rules.
        /// </summary>
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

        /// <summary>
        /// Counts number of possible values for a given coordinate. 
        /// </summary>
        private int CountPoss(int x, int y)
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

        /// <summary>
        /// Builds array with number of possible values for each coordinate.
        /// </summary>
        public void InitializePossGrid()
        {
            possGrid = new Tuple<int, int, int>[n * n];
            int x = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //tpossGrid[i,j] = countPoss(i, j);
                    int val = CountPoss(i, j);
                    possGrid[x] = new Tuple<int, int, int>(val, i, j);
                    x++;
                }
            }
            Array.Sort(possGrid);
        }
    }
}
