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
            sudoku.PrintSudoku();
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

        public Sudoku BT2SolveSudoku(string[] input)
        {
            //implementatie van opdracht 2
            sudoku = new Sudoku(input);
            sudoku.PrintSudoku();
            sudoku.InitializePossGrid();
            BT2Solve(new Operation(0, 0, 0), 0);
            return sudoku;

        }
        private void BT2Solve(Operation lastOperation, int k)
        {
            bool moreChildren = true;
            var parentOperation = lastOperation;
            int xbest = k;
            while(!solved && moreChildren)
            {
                Tuple<Operation,int> result = sudoku.GetNewPossSudoku(lastOperation, xbest);
                var operation = result.Item1;
                //xbest = result.Item2;
                if(operation.val == -2) //solved
                { solved = true; }
                else if (operation.val == -1 || xbest >= sudoku.n*sudoku.n) //dead branch
                { 
                    moreChildren = false;
                    sudoku.UndoLastOperation(parentOperation);
                    xbest--;
                }
                else
                {
                    sudoku.AugmentSudoku(operation);
                    Console.Clear();
                    if (xbest > 70)
                    {
                        sudoku.PrintSudoku();
                        PrintPossGrid();
                        System.Threading.Thread.Sleep(100);
                    }
                    BT2Solve(operation, xbest);
                    lastOperation = operation;
                    xbest++;
                }

            }
        }

        private void PrintPossGrid()
        {
            int[,] possgrid = new int[sudoku.n, sudoku.n];
            foreach (Tuple<int, int, int> T in sudoku.possGrid)
            {
                possgrid[T.Item2, T.Item3] = T.Item1;
            }
            Console.WriteLine("______________");
            for (int i = 0; i < sudoku.n; i++)
            {
                for (int j = 0; j < sudoku.n; j++)
                {
                    Console.Write(possgrid[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }

    class Sudoku
    {
        public int n;
        private int[,] puzzle;
        //public int[,] possGrid;
        public Tuple<int, int, int>[] possGrid;
        private int sqrtN;
        
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
            puzzle[opp.x, opp.y] = 0;
        }

        public void InitializePossGrid()
        {
            possGrid = new Tuple<int, int, int>[n*n];
            int x = 0;
            for (int i = 0; i<n; i++)
            {
                for (int j = 0; j<n; j++)
                {
                    //tpossGrid[i,j] = countPoss(i, j);
                    int val = countPoss(i, j);
                    possGrid[x] = new Tuple<int, int, int>(val, i, j);
                    x++;
                }
            }
            Array.Sort(possGrid);
            
        }

        private int countPoss(int x, int y)
        {
            //square isn't empty
            if (puzzle[x, y] != 0) return 0;

            bool[] bools = new bool[n+1];
            for(int i = 0; i < n+1; i++)
            { bools[i] = false; }
            //check horizontal and vertical lines
            for(int i = 0; i<n; i++)
            {
               bools[puzzle[x, i]] = true;
               bools[puzzle[i, y]] = true;
            }
            //check surrouding square
            int x_block = x - x % sqrtN;
            int y_block = y - y % sqrtN;
            for (int i = x_block; i < x_block + sqrtN; i++)
            {
                for (int j = y_block; j < y_block + sqrtN; j++)
                {
                    bools[puzzle[i, j]] = true;
                }
            }
            int num = n;
            for (int i = 1; i<n+1; i++)
            {
                if (bools[i]) num--;
            }
            return num;
        }

        public Tuple<Operation, int> GetNewPossSudoku(Operation lastOperation, int k)
        {
            int x = possGrid[k].Item2;
            int y = possGrid[k].Item3;
            int val = 0;
            if (lastOperation.x == x && lastOperation.y == y)
            { val = lastOperation.val; }
            val++;
            int xbest = k;
            if (puzzle[x, y] != 0) return new Tuple<Operation, int>(new Operation(x, y, puzzle[x,y]), xbest);
           /*while (true)
           {
                if (puzzle[x,y] != 0)
                {
                    
                    xbest++;
                    if (xbest >= n * n)
                    {
                        if (CheckSudoku())
                        {
                            //solved
                            return new Tuple<Operation, int>(new Operation(x, y, -2), xbest);
                        }

                        else
                        { return new Tuple<Operation, int>(new Operation(x, y, -1), xbest); } //branch dead
                    }
                    x = possGrid[xbest].Item2;
                    y = possGrid[xbest].Item3;
                }
                else
                { break; }
            }*/
            bool found = false;
            while(!found)
            {
                if (val <= n)
                {
                    found = TestOperation(x,y,val);
                    if (!found) val++;
                }
                else
                {
                    if (CheckSudoku())
                    { 
                        //solved
                        return new Tuple<Operation, int>(new Operation(x, y, -2), xbest); 
                    }

                    else
                    { return new Tuple<Operation, int>(new Operation(x, y, -1), xbest); } //branch dead
                }
            }
            /*while (!found)
            {

                if (val <= n)
                {
                    found =TestOperation(x, y, val) ;
                    if (!found) val++;
                }
                else if (xbest > n*n)
                {
                    if (CheckSudoku())
                    { 
                        //solved
                        return new Tuple<Operation, int>(new Operation(x, y, -2), xbest); 
                    }

                    else
                    { return new Tuple<Operation, int>(new Operation(x, y, -1), xbest); } //branch dead
                }
               else
                {
                    xbest++;
                    if (xbest >= n * n) return new Tuple<Operation, int>(new Operation(x, y, -1), xbest); //branch dead
                    x = possGrid[xbest].Item2;
                    y = possGrid[xbest].Item3;
                    val = puzzle[x, y];
                }*/
            //}

            Console.WriteLine(xbest);
            //System.Threading.Thread.Sleep(100);
            return new Tuple<Operation, int>(new Operation(x, y, val), xbest);
        }
    }
}
