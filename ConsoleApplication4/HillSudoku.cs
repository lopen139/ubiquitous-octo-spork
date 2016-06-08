using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public struct Cell
    {
        public int x;
        public int y;

        public Cell(int _x, int _y) : this()
        {
            x = _x;
            y = _y;
        }
    }

    public struct SwapOperation : IComparable<SwapOperation>
    {
        public Cell cell1;
        public Cell cell2;
        public int newFitnessValue;

        public SwapOperation(Cell _cell1, Cell _cell2) : this()
        {
            cell1 = _cell1;
            cell2 = _cell2;
        }

        public int CompareTo(SwapOperation that)
        {
            if (this.newFitnessValue > that.newFitnessValue) return -1;
            if (this.newFitnessValue == that.newFitnessValue) return 0;
            return 1;
        }
    }

    public class HillSudoku : Sudoku
    {
        public int[,] hillpuzzle;
        private int[] fitnessX;
        private int[] fitnessY;
        
        public HillSudoku(int[,] _puzzle, int _n) : base(_puzzle, _n) {}

        public HillSudoku(string[] input) : base(input) {}

        public int TotalFitness()
        {
            return fitnessX.Sum() + fitnessY.Sum();
        }

        /// <summary>
        /// Resets the sudoku to new random instant
        /// </summary>
        public void ResetInstant(Random random)
        {
            RandomizeSudoku(random);
            CalculateFitness();
        }

        /// <summary>
        /// Fills the fitness arrays (from scratch)
        /// </summary>
        public void CalculateFitness()
        {
            fitnessX = new int[n];
            fitnessY = new int[n];
            for (int i = 0; i < n; i++)
            {
                fitnessX[i] = ColumnFitness(i);
                fitnessY[i] = RowFitness(i);
            }
        }


        public bool CompareSudokus(int[,] state)
        {
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    if (state[x, y] != hillpuzzle[x, y]) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a given cell is in conflict with any other cell.
        /// </summary>
        /// <param name="x">RowNumber</param>
        /// <param name="y">ColumnNmber</param>
        /// <returns>true: conflict, false: no conflict</returns>
        public bool CheckConflicts(Cell cell)
        {
            for (int i = 0; i < n; i++)
            {
                if (hillpuzzle[cell.x, cell.y] == hillpuzzle[i, cell.y] && cell.x != i) return true;
                if (hillpuzzle[cell.x, cell.y] == hillpuzzle[cell.x, i] && cell.y != i) return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the fitness arrays
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void UpdateFitness(SwapOperation swap)
        {
            Cell cell1 = swap.cell1;
            Cell cell2 = swap.cell2;
            fitnessX[cell1.x] = ColumnFitness(cell1.x);
            fitnessY[cell1.y] = RowFitness(cell1.y);
            if (cell1.x != cell2.x) fitnessX[cell2.x] = ColumnFitness(cell2.x);
            if (cell1.y != cell2.y) fitnessY[cell2.y] = RowFitness(cell2.y);

        }

        /// <summary>
        /// Calculates the fitness for a given row.
        /// Every number present increases the fitness with 1.
        /// </summary>
        /// <param name="y">RowNumber</param>
        /// <returns>Fitness</returns>
        public int RowFitness(int y)
        {
            bool[] present = new bool[n + 1];
            for (int i = 0; i < n; i++)
            {
                int num = hillpuzzle[i, y];
                present[num] = true;
            }
            int ret = 0;
            for (int i = 1; i < n + 1; i++)
            {
                if (present[i]) ret++;
            }
            return ret;
        }

  

        /// <summary>
        /// Calculates the fitness for a given column.
        /// Every number present increases the fitness with 1.
        /// </summary>
        /// <param name="x">ColumnNumber</param>
        /// <returns>Fitness</returns>
        public int ColumnFitness(int x)
        {
            bool[] present = new bool[n + 1];
            for (int i = 0; i < n; i++)
            {
                int num = hillpuzzle[x, i];
                present[num] = true;
            }
            int ret = 0;
            for (int i = 1; i < n + 1; i++)
            {
                if (present[i]) ret++;
            }
            return ret;
        }

        /// <summary>
        /// Calculates the combined fitness of the columns and rows that are swapped
        /// </summary>
        /// <param name="swap"></param>
        /// <returns>Combines fitness of the 2 rows and 2 columns</returns>
        public int CombinedFitness(SwapOperation swap)
        {
            Cell cell1 = swap.cell1;
            Cell cell2 = swap.cell2;
            //Column(s)
            int x1Fitness = ColumnFitness(cell1.x);
            int x2Fitness = 0;
            if (cell1.x != cell2.x) x2Fitness = ColumnFitness(cell2.x);

            //Row(s)
            int y1Fitness = RowFitness(cell1.y);
            int y2Fitness = 0;
            if (cell1.y != cell2.y) y2Fitness = RowFitness(cell2.y);

            return x1Fitness + x2Fitness + y1Fitness + y2Fitness;
        }

        /// <summary>
        ///  Checks if the sudoku is correctly filled
        /// </summary>
        /// <returns></returns>
        public bool CheckSudoku()
        {
            for (int x = 0; x < n; x++)
            {

                for (int y = 0; y < n; y++)
                {
                    int val = hillpuzzle[x, y];
                    //Test horizontal
                    for (int i = 0; i < n - 1; i++)
                    {
                        if (val == hillpuzzle[i, y]) return false;
                    }
                    //Test vertical
                    for (int i = 0; i < n - 1; i++)
                    {
                        if (val == hillpuzzle[x, i]) return false;
                    }
                    //Test block
                    int x_block = x - x % sqrtN;
                    int y_block = y - y % sqrtN;
                    for (int i = x_block; i < x_block + sqrtN; i++)
                    {
                        for (int j = y_block; j < y_block + sqrtN; j++)
                        {
                            if (val == hillpuzzle[i, j]) return false;
                        }
                    }

                }
            }
            return true;
        }

        public int[,] CopyPuzzle()
        {
            int[,] result = new int[n, n];
            for (int i = 0; i<n; i++)
            {
                for (int j = 0; j<n; j++)
                {
                    result[i,j] = hillpuzzle[i,j];
                }
            }
            return result;
        }

        public void AugmentSudoku(SwapOperation swap)
        {
            Cell cell1 = swap.cell1;
            Cell cell2 = swap.cell2;
            int temp = hillpuzzle[cell1.x,cell1.y];
            hillpuzzle[cell1.x, cell1.y] = hillpuzzle[cell2.x, cell2.y];
            hillpuzzle[cell2.x, cell2.y] = temp;
        }

        public void PrintState()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(hillpuzzle[i, j]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        public void RandomizeSudoku(Random random)
        {
            hillpuzzle = new int[n, n];
            for(int yblock = 1; yblock<=3; yblock++)
            {
                for(int xblock =1; xblock <=3; xblock++)
                {
                    int cornerx = 3 * xblock - 3;
                    int cornery = 3 * yblock - 3;
                    List<int> L = new List<int>();
                    for(int i=1; i <= 9; i++) 
                    { L.Add(i); }
                    
                    for(int i=0; i<3; i++)
                    {
                        for(int j =0; j<3; j++)
                        {
                            L.Remove(puzzle[cornerx + j, cornery + i]);
                            hillpuzzle[cornerx + j, cornery + i] = puzzle[cornerx + j, cornery + i];
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if(hillpuzzle[cornerx+i,cornery+j] == 0)
                            {
                                int r = random.Next(L.Count-1); //List is 0 base
                                hillpuzzle[cornerx + i, cornery + j] = L[r];
                                L.Remove(L[r]);
                            }
                        }
                    }
                }
            }
        }
    }
}
