using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    /// <summary>
    /// Finds local minimum for a given faulty-sudoku instance 
    /// </summary>
    class HillClimber
    {
        public Sudoku state;
        private bool[,] conflictArray;
        private int[] fitnessX;
        private int[] fitnessY;

        public HillClimber(HillSudoku _state)
        {
            state = _state;
            CalculateFitness();
            fillConflictArray();
        }

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

        private int RowFitness(int y)
        {
            bool present = new
            for(int i = 0; i < state.n; i++)
            {

            }
        }

        private int ColumnFitness(int x)
        {
            throw new NotImplementedException();
        }

        private void fillConflictArray()
        {
            for(int x = 0; x < state.n; x++)
            {
                for (int y = 0; y < state.n; y++)
                {
                    conflictArray[x, y] = CheckFonflicts(x, y);
                }
            }
        }

        private bool CheckFonflicts(int x, int y)
        {
            for (int i = 0; i < state.n; i++)
            {
                if (state.puzzle[x, y] == state.puzzle[i, y] && x != i) return false;
                if (state.puzzle[x, y] == state.puzzle[x, i] && y != i) return false;
            }
            return true;
        }

        public void Climb()
        {
            bool minimumFound = false;
            while (!minimumFound)
            {
                for (int x = 0; x < state.n; x++)
                {
                    for (int y = 0; y < state.n; y++)
                    {
                        if (conflictArray[x, y])
                        {
                            for (int i = x + 1; i < state.n; i++)
                            {
                                for (int j = y; j < state.n; j++)
                                {
                                    int deltaF = DeltaFitness(x, y, i, j);
                                    if() 
                                }
                            }
                        }
                    }
                }
                minimumFound = true;
            }
        }
    }
}
