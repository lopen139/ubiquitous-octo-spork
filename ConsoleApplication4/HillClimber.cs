using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace ConsoleApplication4
{
    public class HillClimber
    {
        public HillSudoku state;
        public List<long> stepsList;
        public List<long> ticksList;
        public List<long> msList;
        public long steps;
        public long restarts;
        public Random random;
        public long solveTime;
        public long solveTicks;

        public HillClimber(HillSudoku _state)
        {
            state = _state;
            steps = 0;
            restarts = 0;
            stepsList = new List<long>();
            ticksList = new List<long>();
            msList = new List<long>();
        }
        
        //Analysis
        public double averageSteps => stepsList.Average();
        public double sdSteps => Program.StandardDeviation(stepsList);
        public double averageTicks => ticksList.Average();
        public double sdTicks => Program.StandardDeviation(ticksList);
        public double averageMilliseconds => msList.Average();
        public double sdMilliseconds => Program.StandardDeviation(msList);

        /// <summary>
        /// Resets all the Analysis parameters
        /// </summary>
        public void ResetAnalysisParameters()
        {
            steps = 0;
            solveTicks = 0;
            solveTime = 0;
            restarts = 0;
            stepsList = new List<long>();
            ticksList = new List<long>();
            msList = new List<long>();
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
            state.CalculateFitness();
            if (print) Console.WriteLine("Begin Iterated Local Search");
            int best = 0;
            bool solutionFound = false;
            int steps = 1;

            HillClimb();
            int[,] bestSudoku = state.CopyPuzzle();
            if (state.TotalFitness() == 2 * state.n * state.n) solutionFound = true;

            while (!solutionFound)
            {
                steps++;

                state.hillpuzzle = bestSudoku;
                RandomWalk(k, print);
                HillClimb();
                //if (print)
                //{
                //    Console.WriteLine("Hill Climb:");
                //    state.PrintState();
                //    Console.WriteLine("-----------");
                //}

                int fit = state.TotalFitness();

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
                state.PrintState();
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
                Cell cell1 = new Cell(random.Next(0, state.n), random.Next(0, state.n));

                int x_block = cell1.x - (cell1.x % state.sqrtN);
                int y_block = cell1.y - (cell1.y % state.sqrtN);

                Cell cell2 = new Cell(x_block + random.Next(0, state.sqrtN), y_block + random.Next(0, state.sqrtN));

                SwapOperation swap = new SwapOperation(cell1, cell2);

                while (cell1.x == cell2.x && cell1.y == cell2.y) {
                    cell2.x = x_block + random.Next(0, state.sqrtN);
                    cell2.y = y_block + random.Next(0, state.sqrtN);
                }

                state.AugmentSudoku(swap);
            }
            //if (print)
            //{
            //    Console.WriteLine("Random walk:");
            //    state.PrintState();
            //    Console.WriteLine("------------");
            //}
        }

        public void RandomRestartHillClimb(Random _random, bool print = false, int maxRestarts = int.MaxValue)
        {
            if(print) Console.WriteLine("Begin Random-restart Hill-climbing");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int best = 0;
            random = _random;
            bool solutionFound = false;
            while (!solutionFound)
            {
                restarts++;
                state.ResetInstant(random);
                HillClimb();

                int fit = state.TotalFitness();
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
            solveTicks = watch.ElapsedTicks;
            solveTime = watch.ElapsedMilliseconds;
            if (print && !solutionFound) Console.WriteLine("No solution found in {0} restarts.", maxRestarts);
            if (print && solutionFound)
            {
                Console.WriteLine("Solution found:");
                state.PrintState();
                Console.WriteLine("---------------");
            }
        }

        /// <summary>
        /// Finds local maximum. Algorithm is deterministic.
        /// </summary>
        public void HillClimb()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            bool swapped = true;
            while (swapped)
            {
                swapped = FindConflict();
                steps++;
            }
            watch.Stop();
            ticksList.Add(watch.ElapsedTicks);
            msList.Add(watch.ElapsedMilliseconds);
            stepsList.Add(steps);
            steps = 0;
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
                    Cell cell = new Cell(x,y);
                    if (state.CheckConflicts(cell))
                    {
                       if(SwapCell(cell)) return true;
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
        public bool SwapCell(Cell cell)
        {
            int x_block = cell.x - (cell.x % state.sqrtN);
            int y_block = cell.y - (cell.y % state.sqrtN);
            for (int i = x_block; i < x_block + state.sqrtN; i++)
            {
                for (int j = y_block; j < y_block + state.sqrtN; j++)
                {
                    Cell cell2 = new Cell(i,j);
                    SwapOperation swap = new SwapOperation(cell, cell2);
                    if (state.CheckConflicts(cell2) && FitnessImproved(swap)) return true;
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
        public bool FitnessImproved(SwapOperation swap)
        {
            //Calculate current fitness
            int combinedFitness = state.CombinedFitness(swap);
            
            //Temporarily swap:
            state.AugmentSudoku(swap);

            //Calculate new fitness
            int NewcombinedFitness = state.CombinedFitness(swap);

            if (NewcombinedFitness > combinedFitness)
            {
                state.UpdateFitness(swap);
                return true;
            }
            else
            {
                //Swap back:
                state.AugmentSudoku(swap);
                return false;
            }
        }
    }
}
