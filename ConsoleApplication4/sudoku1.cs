using System.Diagnostics;

namespace ConsoleApplication4
{
    /// <summary>
    /// Solver class for first assigment
    /// </summary>
    public class SudokuSolver1
    {
        public Sudoku1 sudoku;
        private bool solved;
        public long steps = 0;
        public long solveTime;
        public long solveTicks;

        public Sudoku SolveSudoku(string[] input)
        {
            sudoku = new Sudoku1(input);
            sudoku.PrintSudoku();
            Solve(new Operation(0, 0, 0));
            return sudoku;
        }
        public Sudoku SolveSudoku(int[,] input, int n)
        {
            sudoku = new Sudoku1(input, n);
            //sudoku.PrintSudoku();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Solve(new Operation(0, 0, 0));
            watch.Stop();
            solveTime = watch.ElapsedMilliseconds;
            solveTicks = watch.ElapsedTicks;

            return sudoku;
        }

        public void Solve(Operation lastOperation)
        {
            steps++;

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
                    sudoku.UndoLastOperation(parentOperation);
                }
                else
                {
                    sudoku.AugmentSudoku(operation);
                    Solve(operation);
                    lastOperation = operation;
                }
            }
        }
    }

    /// <summary>
    /// Sudoku class for assignment 1 
    /// </summary>
    public class Sudoku1 : Sudoku
    {
        public Sudoku1(int[,] _puzzle, int _n) : base(_puzzle, _n) {}

        public Sudoku1(string[] input) : base(input) {}

        /// <summary>
        /// Generates the next operation that can be applied.
        /// </summary>
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
                    return new Operation(0, 0, -1); // Branch is dead
                }
                else if (y < n - 1)
                {
                    y++;
                    val = 0;
                }
                else if (x < n - 1)
                {
                    x++;
                    y = 0;
                    val = 0;
                }
                else
                {
                    return new Operation(0, 0, -2); // Answer found
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
            int x_block = x - x%sqrtN;
            int y_block = y - y%sqrtN;
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
