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
        private List<SwapOperation> successors;

        public TabuSearch(HillSudoku _state, int _tabuSize)
        {
            state = _state;
            tabuSize = _tabuSize;
            steps = 0;
            restarts = 0;
            tabuList = new Queue<int[,]>();
            random = new Random();
            state.ResetInstant(random);
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
                    successors = new List<SwapOperation>();
                    if (tabuList.Count >= tabuSize) tabuList.Dequeue();
                    tabuList.Enqueue(state.CopyPuzzle());
                    if(!FindConflict())
                    {
                        successors.Sort();
                        SwapOperation s = successors[0];
                        state.AugmentSudoku(s);
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
        public bool SwapCell(Cell cell)
        {
            int x_block = cell.x - (cell.x % state.sqrtN);
            int y_block = cell.y - (cell.y % state.sqrtN);
            for (int i = x_block; i < x_block + state.sqrtN; i++)
            {
                for (int j = y_block; j < y_block + state.sqrtN; j++)
                {
                    Cell cell2 = new Cell(i,j);
                    if (state.CheckConflicts(cell2) && FitnessImproved(new SwapOperation(cell, cell2)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool FitnessImproved(SwapOperation swap)
        {
            //Calculate current fitness
            int combinedFitness = state.CombinedFitness(swap);

            //Temporarily swap:
            state.AugmentSudoku(swap);

            bool tabus = false;
            foreach (int[,] tabu in tabuList)
            {
                if (tabu.OfType<int>().SequenceEqual(state.hillpuzzle.OfType<int>()))
                {
                    tabus = true;
                }
            }
            //Calculate new fitness
            int NewcombinedFitness = state.CombinedFitness(swap);

            if (NewcombinedFitness > combinedFitness && !tabus)
            {
                state.UpdateFitness(swap);
                return true;
            }
            else
            {
                //Swap back:

                /*if (!tabuList.Contains(state.hillpuzzle))
                {
                    //add the succesor to the list of possible successors
                    successors.Add(new Tuple<int, Tuple<int, int>, Tuple<int, int>>(NewcombinedFitness, new Tuple<int, int>(x1, y1), new Tuple<int, int>(x2, y2)));
                }*/
                if (!tabus)
                {
                    successors.Add(swap);
                }
                else { int huh = 0; }

                    state.AugmentSudoku(swap);
                return false;
            }
        }

        public bool FindConflict()
        {
            for (int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    Cell cell = new Cell(x,y);
                    if (state.CheckConflicts(cell))
                    {
                        if (SwapCell(cell)) return true;
                    }
                }
            }
            return false;
        }

    }
}
