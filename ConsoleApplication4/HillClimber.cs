using System;
using System.Linq;

namespace ConsoleApplication4
{

    public struct Cell
    {
        int x;
        int y;
    }

    public struct SwapOperation
    {
        Cell cell1;
        Cell cell2;
        int newFitnessValue;
    }

    /// <summary>
    /// Finds local minimum for a given faulty-sudoku instance 
    /// </summary>
    public class HillClimber
    {
        public HillSudoku state;
        public bool[,] conflictArray;
        private int[] fitnessX;
        private int[] fitnessY;
        public int steps;
        public int restarts;
        public Random random;
        public int solveTime;
        public int solveTicks;

        public HillClimber(HillSudoku _state)
        {
            state = _state;
            steps = 0;
            restarts = 0;
        }

        /// <summary>
        /// Nonadaptive Iterative local search
        /// </summary>
        /// <param name="_random"></param>
        /// <param name="k">Number of Random-Walk steps</param>
        /// <param name="print"></param>
        public void IteratedLocalSearch(Random _random, int k, bool print = false, int maxSteps = -1)
        {
            random = _random;
            state.RandomizeSudoku(random);
            CalculateFitness();
            if (print) Console.WriteLine("Begin Iterated Local Search");
            int best = 0;
            bool solutionFound = false;
            int steps = 1;

            HillClimb();
            int[,] bestSudoku = state.CopyPuzzle();
            if (TotalFitness() == 2 * state.n * state.n) solutionFound = true;

            while (!solutionFound)
            {
                steps++;

                state.hillpuzzle = bestSudoku;
                RandomWalk(k, print);
                HillClimb();
                if (print)
                {
                    Console.WriteLine("Hill Climb:");
                    state.PrintSudoku();
                    Console.WriteLine("-----------");
                }

                int fit = TotalFitness();

                if (print)
                {
                    if (fit > best)
                    {
                        Console.WriteLine("new best fit: {0}. run: {1}", fit, steps);
                    }
                }

                if (fit == 2 * state.n * state.n) solutionFound = true;
                else if(fit > best)
                {
                    best = fit;
                    bestSudoku = state.CopyPuzzle();
                }
                if (steps == maxSteps) break;
            }
            if (print && solutionFound)
            {
                Console.WriteLine("Solution found in {0} steps:", steps);
                state.PrintSudoku();
                Console.WriteLine("---------------");
            }
        }

        /// <summary>
        /// Performs random walk.
        /// </summary>
        /// <param name="k">number of steps</param>
        public void RandomWalk(int k, bool print = false)
        {
            for(int i = 0; i < k; i++)
            {
                int x1 = random.Next(0, state.n);
                int y1 = random.Next(0, state.n);

                int x_block = x1 - (x1 % state.sqrtN);
                int y_block = y1 - (y1 % state.sqrtN);

                int x2 = x_block + random.Next(0, state.sqrtN);
                int y2 = y_block + random.Next(0, state.sqrtN);

                while (x1 == x2 && y1 == y2) {
                    x2 = x_block + random.Next(0, state.sqrtN);
                    y2 = y_block + random.Next(0, state.sqrtN);
                }

                state.AugmentSudoku(x1, y1, x2, y2);
            }
            if (print)
            {
                Console.WriteLine("Random walk:");
                state.PrintSudoku();
                Console.WriteLine("------------");
            }
        }

        public void RandomRestartHillClimb(Random _random, bool print = false, int maxRestarts = int.MaxValue)
        {
            if(print) Console.WriteLine("Begin Random-restart Hill-climbing");
            int best = 0;
            random = _random;
            bool solutionFound = false;
            while (!solutionFound)
            {
                restarts++;
                ResetInstant();
                HillClimb();
                int fit = TotalFitness();
                if (print)
                {
                    if (fit > best)
                    {
                        best = fit;
                        Console.WriteLine("new best fit: {0}. run: {1}",best,restarts);
                    }
                }
                if (fit == 2*state.n*state.n) solutionFound = true;
                if (restarts == maxRestarts) break;
            }
            if (print && !solutionFound) Console.WriteLine("No solution found in {0} restarts.", maxRestarts);
            if (print && solutionFound)
            {
                Console.WriteLine("Solution found:");
                state.PrintSudoku();
                Console.WriteLine("---------------");
            }
        }

        public int TotalFitness()
        {
            return fitnessX.Sum() + fitnessY.Sum();
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
        public int RowFitness(int y)
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
        public int ColumnFitness(int x)
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
        public void FillConflictArray()
        {
            for(int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    conflictArray[x, y] = CheckConflicts(x, y);
                }
            }
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
        /// Finds local maximum. Algorithm is deterministic.
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
        public bool FindConflict()
        {
            for (int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    if (CheckConflicts(x,y))
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
        public bool SwapCell(int x, int y)
        {
            int x_block = x - (x % state.sqrtN);
            int y_block = y - (y % state.sqrtN);
            for (int i = x_block; i < x_block + state.sqrtN; i++)
            {
                for (int j = y_block; j < y_block + state.sqrtN; j++)
                {
                    if (CheckConflicts(i, j) && FitnessImproved(x, y, i, j)) return true;
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
        public bool FitnessImproved(int x1, int y1, int x2, int y2)
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
    }
}
