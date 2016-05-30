using System;

namespace ConsoleApplication4
{
    /// <summary>
    /// Finds local minimum for a given faulty-sudoku instance 
    /// </summary>
    class HillClimber
    {
        public HillSudoku state;
        private bool[,] conflictArray;
        private int[] fitnessX;
        private int[] fitnessY;
        private int steps;

        public HillClimber(HillSudoku _state)
        {
            state = _state;
            CalculateFitness();
            //FillConflictArray();
            steps = 0;
        }

        /// <summary>
        /// Fills the fitness arrays (from scratch)
        /// </summary>
        private void CalculateFitness()
        {
            fitnessX = new int[state.n];
            fitnessY = new int[state.n];
            for(int i = 0; i < state.n; i++)
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
        private int RowFitness(int y)
        {
            bool[] present = new bool[state.n + 1];
            for(int i = 0; i < state.n; i++)
            {
                int num = state.hillpuzzle[i,y];
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
        private int ColumnFitness(int x)
        {
            bool[] present = new bool[state.n + 1];
            for (int i = 0; i < state.n; i++)
            {
                int num = state.hillpuzzle[x,i];
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
        /// Calculate, for each cell, if there is a conflict (from scratch).
        /// </summary>
        private void FillConflictArray()
        {
            for(int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    conflictArray[x, y] = CheckFonflicts(x, y);
                }
            }
        }

        /// <summary>
        /// Checks if a given cell is in conflict with any other cell.
        /// </summary>
        /// <param name="x">RowNumber</param>
        /// <param name="y">ColumnNmber</param>
        /// <returns>conflict</returns>
        private bool CheckFonflicts(int x, int y)
        {
            for (int i = 0; i < state.n; i++)
            {
                if (state.hillpuzzle[x, y] == state.hillpuzzle[i, y] && x != i) return false;
                if (state.hillpuzzle[x, y] == state.hillpuzzle[x, i] && y != i) return false;
            }
            return true;
        }

        /// <summary>
        /// Finds local maximum
        /// </summary>
        public void HillClimb()
        {
            bool swapped = true;
            while (swapped)
            {
                swapped = FindConflict();
                steps++;
            }
        }

        /// <summary>
        /// Looks for conflict and makes a swap
        /// </summary>
        /// <returns>true: swap was made , false: no imroving swaps</returns>
        private bool FindConflict()
        {
            for (int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    if (CheckFonflicts(x,y))
                    {
                       if(SwapCell(x, y)) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Swaps given cell with other cell (in the same block) that has conflicts.
        /// Will only swap when fitness increases.
        /// </summary>
        /// <param name="x">RowNumber</param>
        /// <param name="y">ColumnNumber</param>
        /// <returns>true: swap was made, false: no improving swaps</returns>
        private bool SwapCell(int x, int y)
        {
            int x_block = x - (x % state.sqrtN);
            int y_block = y - (y % state.sqrtN);
            for (int i = x_block; i < x_block + state.sqrtN; i++)
            {
                for (int j = y_block; j < y_block + state.sqrtN; j++)
                {
                    if (CheckFonflicts(i, j) && FitnessImproved(x, y, i, j)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if swap increases the fitness. Also updates:
        /// 1: puzzle state
        /// 2: Fitness array
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>true: swap increases fitness, false: swap does not increase fitness</returns>
        private bool FitnessImproved(int x1, int y1, int x2, int y2)
        {
            //Calculate current fitness
            int combinedFitness = CombinedFitness(x1, y1, x2, y2);
            
            //Temporarily swap:
            state.AugmentSudoku(x1, y1, x2, y2);

            //Calculate new fitness
            int NewcombinedFitness = CombinedFitness(x1, y1, x2, y2);

            if (NewcombinedFitness > combinedFitness)
            {
                UpdateFitness(x1, y1, x2, y2);
                return true;
            }
            else
            {
                //Swap back:
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
        private void UpdateFitness(int x1, int y1, int x2, int y2)
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
        private int CombinedFitness(int x1, int y1, int x2, int y2)
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
    }
}
