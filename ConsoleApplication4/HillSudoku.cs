using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    public class HillSudoku : Sudoku
    {
        public int[,] hillpuzzle;

        public HillSudoku(int[,] _puzzle, int _n) : base(_puzzle, _n) {}

        public HillSudoku(string[] input) : base(input) {}

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

        public void AugmentSudoku(int x1,int y1,int x2,int y2)
        {
            int temp = hillpuzzle[x1,y1];
            hillpuzzle[x1, y1] = hillpuzzle[x2, y2];
            hillpuzzle[x2, y2] = temp;
        }
        public void PrintSudoku()
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
