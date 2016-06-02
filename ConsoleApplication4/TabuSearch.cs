using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public class TabuSearch
    {
        public HillSudoku state;
        private bool[,] conflictArray;
        private int[] fitnessX;
        private int[] fitnessY;
        public int steps;
        public int restarts;
        public Random random;
        public int solveTime;
        public int solveTicks;
        public Queue<int[,]> tabuList;
        private int tabuSize;
        private List<Tuple<int, Tuple<int, int>, Tuple<int, int>>> successors;

        public TabuSearch(HillSudoku _state, int _tabuSize)
        {
            state = _state;
            tabuSize = _tabuSize;
            steps = 0;
            restarts = 0;
            tabuList = new Queue<int[,]>();
            random = new Random();
            ResetInstant();
        }

        public void Search()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            bool solved = false;
            while(!solved)
            {

                if (state.CheckSudoku())
                {
                    solved = true;
                }
                else
                {
                    steps++;
                    successors = new List<Tuple<int, Tuple<int, int>, Tuple<int, int>>>();
                    if (tabuList.Count >= tabuSize) tabuList.Dequeue();
                    tabuList.Enqueue(state.CopyPuzzle());
                    if(!FindConflict())
                    {
                        successors.Sort();
                        var s = successors[0];
                        state.AugmentSudoku(s.Item2.Item1, s.Item2.Item2, s.Item3.Item1, s.Item3.Item2);
                    }
                    
                }
            }
            stopwatch.Stop();
            solveTime = (int)stopwatch.ElapsedMilliseconds;
            solveTicks = (int)stopwatch.ElapsedTicks;
        }

        /// <summary>
        /// Swaps given cell with other cell (in the same block) that has conflicts.
        /// Will only swap when fitness increases.
        /// </summary>
        /// <param name="x">RowNumber</param>
        /// <param name="y">ColumnNumber</param>
        public bool SwapCell(int x, int y)
        {
            int x_block = x - (x % state.sqrtN);
            int y_block = y - (y % state.sqrtN);
            for (int i = x_block; i < x_block + state.sqrtN; i++)
            {
                for (int j = y_block; j < y_block + state.sqrtN; j++)
                {
                    if (CheckConflicts(i, j) && FitnessImproved(x, y, i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool FitnessImproved(int x1, int y1, int x2, int y2)
        {
            //Calculate current fitness
            int combinedFitness = CombinedFitness(x1, y1, x2, y2);

            //Temporarily swap:
            state.AugmentSudoku(x1, y1, x2, y2);

            //Calculate new fitness
            int NewcombinedFitness = CombinedFitness(x1, y1, x2, y2);

            if (NewcombinedFitness > combinedFitness && !tabuList.Contains(state.hillpuzzle))
            {
                UpdateFitness(x1, y1, x2, y2);
                return true;
            }
            else
            {
                //Swap back:

                if (!tabuList.Contains(state.hillpuzzle))
                {
                    //add the succesor to the list of possible successors
                    successors.Add(new Tuple<int, Tuple<int, int>, Tuple<int, int>>(NewcombinedFitness, new Tuple<int, int>(x1, y1), new Tuple<int, int>(x2, y2)));
                }
                state.AugmentSudoku(x1, y1, x2, y2);
                return false;
            }
        }

        /// <summary>
        /// Updates the fitness arrays
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void UpdateFitness(int x1, int y1, int x2, int y2)
        {
            fitnessX[x1] = ColumnFitness(x1);
            fitnessY[y1] = RowFitness(y1);
            if (x1 != x2) fitnessX[x2] = ColumnFitness(x2);
            if (y1 != y2) fitnessY[y2] = RowFitness(y2);

        }

        /// <summary>
        /// Calculates the combined fitness of the columns and rows that are swapped
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Combines fitness of the 2 rows and 2 columns</returns>
        public int CombinedFitness(int x1, int y1, int x2, int y2)
        {
            //Column(s)
            int x1Fitness = ColumnFitness(x1);
            int x2Fitness = 0;
            if (x1 != x2) x2Fitness = ColumnFitness(x2);

            //Row(s)
            int y1Fitness = RowFitness(y1);
            int y2Fitness = 0;
            if (y1 != y2) y2Fitness = RowFitness(y2);

            return x1Fitness + x2Fitness + y1Fitness + y2Fitness;
        }

        /// <summary>
        /// Checks if a given cell is in conflict with any other cell.
        /// </summary>
        /// <param name="x">RowNumber</param>
        /// <param name="y">ColumnNmber</param>
        /// <returns>true: conflict, false: no conflict</returns>
        public bool CheckConflicts(int x, int y)
        {
            for (int i = 0; i < state.n; i++)
            {
                if (state.hillpuzzle[x, y] == state.hillpuzzle[i, y] && x != i) return true;
                if (state.hillpuzzle[x, y] == state.hillpuzzle[x, i] && y != i) return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the sudoku to new random instant
        /// </summary>
        public void ResetInstant()
        {
            state.RandomizeSudoku(random);
            CalculateFitness();
            steps = 0;
        }

        /// <summary>
        /// Fills the fitness arrays (from scratch)
        /// </summary>
        public void CalculateFitness()
        {
            fitnessX = new int[state.n];
            fitnessY = new int[state.n];
            for (int i = 0; i < state.n; i++)
            {
                fitnessX[i] = ColumnFitness(i);
                fitnessY[i] = RowFitness(i);
            }
        }

        /// <summary>
        /// Calculates the fitness for a given row.
        /// Every number present increases the fitness with 1.
        /// </summary>
        /// <param name="y">RowNumber</param>
        /// <returns>Fitness</returns>
        public int RowFitness(int y)
        {
            bool[] present = new bool[state.n + 1];
            for (int i = 0; i < state.n; i++)
            {
                int num = state.hillpuzzle[i, y];
                present[num] = true;
            }
            int ret = 0;
            for (int i = 1; i < state.n + 1; i++)
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
            bool[] present = new bool[state.n + 1];
            for (int i = 0; i < state.n; i++)
            {
                int num = state.hillpuzzle[x, i];
                present[num] = true;
            }
            int ret = 0;
            for (int i = 1; i < state.n + 1; i++)
            {
                if (present[i]) ret++;
            }
            return ret;
        }

        public bool FindConflict()
        {
            for (int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    if (CheckConflicts(x, y))
                    {
                        if (SwapCell(x, y)) return true;
                    }
                }
            }
            return false;
        }

    }
}
