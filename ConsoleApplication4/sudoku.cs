using System;

namespace ConsoleApplication4
{

    struct Operation
    {
        public int x;
        public int y;
        public int val;

        /// <summary>
        /// zero if nothing was choosen yet, one if last choice was the best, two is last choicse was second best, etc..
        /// </summary>
        public int lastChoosenBest;

        public Operation(int _x, int _y, int _val, int _lastChoosenBest)
        {
            x = _x;
            y = _y;
            val = _val;
            lastChoosenBest = _lastChoosenBest;
        }
    }

    internal class SudokuSolver
    {
        public Sudoku sudoku;
        private bool solved;

        public Sudoku SolveSudoku(string[] input)
        {
            sudoku = new Sudoku(input);
            Console.WriteLine("--------UNSOLVED:--------");
            sudoku.PrintSudoku();
            Console.WriteLine("--------SOLVED:--------");
            Solve(new Operation(0,0,0,0));
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
                    Console.Clear();
                    Console.WriteLine("SUDOKU:");
                    sudoku.PrintSudoku();
                    Console.WriteLine("POSSIBLEDIGITS:");
                    sudoku.PrintSudoku(true);
                    Console.WriteLine("OPERATION");
                    Console.WriteLine(@"RES:: x: {0}, y: {1}, val: {2}, k: {3} ", operation.x, operation.y, operation.val, operation.lastChoosenBest);
                    System.Threading.Thread.Sleep(300);
                }
                else
                {
                    sudoku.AugmentSudoku(operation);
                    Console.Clear();
                    Console.WriteLine("SUDOKU:");
                    sudoku.PrintSudoku();
                    Console.WriteLine("POSSIBLEDIGITS:");
                    sudoku.PrintSudoku(true);
                    Console.WriteLine("OPERATION");
                    Console.WriteLine(@"AUG:: x: {0}, y: {1}, val: {2}, k: {3} ",operation.x, operation.y, operation.val, operation.lastChoosenBest);
                    System.Threading.Thread.Sleep(300);
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

        /// <summary>
        /// Array that holds the number of possible digits that are still a valid option for each coördinate
        /// </summary>
        private int[,] possibleEntries;
        
        public Sudoku(string[] input)
        {
            n = input.Length;
            puzzle = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    string numstr = input[i][j].ToString();
                    puzzle[i, j] = int.Parse(numstr);
                }
            }
            sqrtN = Convert.ToInt32(Math.Sqrt(n));
            ConstructPossibleEntries();
        }

        /// <summary>
        /// Generates the possibleEntries array from scratch
        /// </summary>
        public void ConstructPossibleEntries()
        {
            possibleEntries = new int[n, n];
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    if (puzzle[x, y] != 0) possibleEntries[x, y] = 0;
                    else possibleEntries[x, y] = CountPossibleEntries(x,y);
                }
            }       
        }

        /// <summary>
        /// Counts possible entries for a given coördinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CountPossibleEntries(int x, int y)
        {
            bool[] bools = new bool[n + 1];
            for (int i = 0; i < n; i++)
            {
                bools[puzzle[x, i]] = true;
                bools[puzzle[i, x]] = true;
            }
            int num = n + 1;
            for (int i = 1; i < n + 1; i++)
            {
                if (bools[i]) num--;
            }
            return num;
        }

        /// <summary>
        /// Subtracts 1 from the column and row of coördinate (x,y). Only call this method when a puzzle entrie is changed from 0 to i in 1..n, where the resulting puzzle is a valid sudoku.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void UpdatePossibleEntries(int x, int y, bool reset = false)
        {
            for (int i = 0; i < n; i++)
            {
                if (!reset)
                {
                    possibleEntries[x, i]--;
                    possibleEntries[i, y]--;
                    if (possibleEntries[x, i] < 0) possibleEntries[x, i] = 0; //Prevents minus signs in possibleEntries
                    if (possibleEntries[i, y] < 0) possibleEntries[i, y] = 0;
                }
                else
                {
                    if (x == i) continue;
                    possibleEntries[x, i]++;
                    possibleEntries[i, y]++;
                }
            }
            if (!reset) possibleEntries[x, y] = 0;
            else possibleEntries[x, y] = CountPossibleEntries(x, y);
        }

        /// <summary>
        /// Prints the puzzle.
        /// </summary>
        public void PrintSudoku(bool printPossibleEntries = false)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if(!printPossibleEntries) Console.Write(puzzle[i, j]);
                    else Console.Write(possibleEntries[i,j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public Operation GetNewSudoku(Operation lastOperation)
        {
            int lastChoosenBest = lastOperation.lastChoosenBest;
            while (true)
            {
                lastChoosenBest++;
                if(lastChoosenBest > n*n) return new Operation(0, 0, -1, 0); //Branch dead
                int[] c = FindKBestPossibleEntrie(lastChoosenBest);
                if (c[0] == -1 || c[1] == -1)
                {
                    //Branch dead
                    return new Operation(0, 0, -1, 0);
                }
                if (c[0] == -2 || c[1] == -2)
                {
                    //Answer found
                    return new Operation(0, 0, -2, 0);
                }
                for (int val = 1; val < n + 1; val++)
                {
                    if (TestOperation(c[0], c[1], val)) return new Operation(c[0], c[1], val, lastChoosenBest);
                }
            }           
        }

        /// <summary>
        /// SLOW method of finding k best possible entrie
        /// </summary>
        /// <param name="k">1..N^2</param>
        /// <returns>array with two elements: [0] = x-coördinate, [1] = y-coördinate</returns>
        private int[] FindKBestPossibleEntrie(int k)
        {
            int[][] bestEntries =  new int[k][];
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    if (possibleEntries[x, y] == 0) continue;
                    for (int i = 0; i < k; i++)
                    {
                        if (bestEntries[i] == null)
                        {
                            bestEntries[i] = new[] {x, y};
                            break;
                        }
                        if (possibleEntries[bestEntries[i][0], bestEntries[i][1]] >= possibleEntries[x, y])
                        {
                            //shift array:
                            for (int j = k - 1; j > i; j--)
                            {
                                if (bestEntries[j] == null) continue;
                                bestEntries[j] = bestEntries[j - 1];
                            }
                            //insert into array:
                            bestEntries[i][0] = x;
                            bestEntries[i][1] = y;
                        }
                    }
                }
            }
            if (possibleEntries[bestEntries[0][0], bestEntries[0][1]] == 0) return new [] { -2, -2 }; //Answer found if best answer is zero
            if (bestEntries[k - 1] == null || possibleEntries[bestEntries[k - 1][0], bestEntries[k - 1][1]] == 0) return new[] { -1, -1 }; //Branch dead if best answer is not zero, but k best answer is zero.
            return bestEntries[k - 1];
        }

        /// <summary>
        /// Applies operation to puzzle and updates the possibleEntries array. Only call this method when resulting puzzle is a valid sudoku.
        /// </summary>
        /// <param name="opp">Operation done on the sudoku</param>
        public void AugmentSudoku(Operation opp)
        {
            if (puzzle[opp.x, opp.y] != 0) throw new Exception("Augmenting on position already occupied");
            puzzle[opp.x, opp.y] = opp.val;
            UpdatePossibleEntries(opp.x, opp.y);
        }

        private bool TestOperation(int x, int y, int val)
        {
            if (puzzle[x, y] != 0) return false;
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
            if (puzzle[opp.x, opp.y] == 0) throw new Exception("Undoing on position not filled");
            puzzle[opp.x, opp.y] = 0;
            UpdatePossibleEntries(opp.x, opp.y, true);
        }
    }
}
