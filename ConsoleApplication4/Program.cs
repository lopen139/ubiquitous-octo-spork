using System;
using System.Collections.Generic;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "..\\..\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku h = new HillSudoku(input[0], 9);
            h.RandomizeSudoku();
            h.PrintSudoku();
            h.RandomizeSudoku();
            h.PrintSudoku();
            //Sudoku Set 1
           // Console.WriteLine("<>-------- Solving p096_sudoku with method 1 --------<>");
            //RunTest("p096_sudoku", 1);
            //Console.WriteLine("<>-------- Solving p096_sudoku with method 2 --------<>");
            //RunTest("p096_sudoku", 2);
            //Sudoku Set 2: hard
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 1 --------<>");
            //RunTest("su17ExtremeDiff500", 1);
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 2 --------<>");
            //RunTest("su17ExtremeDiff500", 2);

            Console.ReadLine();
        }

        public static void RunTest(string fileName, int mode)
        {
            var input = System.IO.File.ReadAllLines("..\\..\\puzzles\\" + fileName + ".txt");
            List<int[,]> parsed;
            if (fileName == "p096_sudoku") parsed = Parser.Parser_p096(input);
            else if (fileName == "su17ExtremeDiff500") parsed = Parser.Parser_su17(input);
            else throw new Exception("no valid filename");
            int i = 0;
            int n = 9;
            string write = "#sudoku #steps  #time   #ticks" + Environment.NewLine;
            foreach (var test1 in parsed)
            {
                Console.WriteLine(@"Sudoku {0} solving...", i);
                if (mode == 1)
                {
                    SudokuSolver1 solver1 = new SudokuSolver1();
                    solver1.SolveSudoku(test1, n);
                    write += i + " " + solver1.steps + "    " + solver1.solveTime + "    " + solver1.solveTicks +
                             Environment.NewLine;
                    solver1.sudoku.PrintSudoku();
                }
                if (mode == 2)
                {
                    SudokuSolver2 solver = new SudokuSolver2();
                    solver.SolveSudoku(test1, n);
                    write += i + " " + solver.steps + "    " + solver.solveTime + "    " + solver.solveTicks +
                             Environment.NewLine;
                    solver.sudoku.PrintSudoku();
                }
                i++;
                if(i == 50) break;
            }
            string loc = "..\\..\\puzzles\\out" + mode + "_" + fileName + ".txt";
            System.IO.File.WriteAllText(loc, write);
            Console.WriteLine(@"Output saved to file: " + loc);
        }
    }

    public static class Parser
    {
        public static List<int[,]> Parser_p096(string[] input)
        {
            List<int[,]> output = new List<int[,]>();
            bool newPuzzle = true;
            int n_puzzle = 0;
            int n = 9;
            while (newPuzzle)
            {
                try
                {
                    var temp = input[10*(n_puzzle + 1)];
                }
                catch
                {
                    newPuzzle = false;
                }
                int[,] puzzle = new int[n, n];
                int i = 0;
                for (int line = 10*n_puzzle + 1; line < 10*(n_puzzle + 1); line++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        string numstr = input[line][j].ToString();
                        puzzle[i, j] = int.Parse(numstr);
                    }
                    i++;
                }
                output.Add(puzzle);
                n_puzzle++;
            }
            return output;
        }

        internal static List<int[,]> Parser_su17(string[] input)
        {
            List<int[,]> output = new List<int[,]>();
            bool newPuzzle = true;
            int n_puzzle = 0;
            int n = 9;
            while (newPuzzle)
            {
                try
                {
                    var temp = input[2*(n_puzzle + 1)];
                }
                catch
                {
                    newPuzzle = false;
                }
                int[,] puzzle = new int[n, n];
                string inp = input[2*n_puzzle];
                inp = inp.Substring(0, n*n);
                int i = 0;
                for (int x = 0; x < n; x++)
                {
                    for (int y = 0; y < n; y++)
                    {
                        puzzle[x, y] = int.Parse(inp[i].ToString());
                        i++;
                    }
                }
                output.Add(puzzle);
                n_puzzle++;
            }
            return output;
        }
    }
}