using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ConsoleApplication4;
using System.Linq;

namespace Tester
{
    [TestClass]
    public class Analyzer
    {
        //[TestMethod]
        public void Test1()
        {
            //RunTest("p096_sudoku", 1);
            RunTest("p096_sudoku", 2);
            //RunTest("su17ExtremeDiff500", 1);
            //RunTest("su17ExtremeDiff500", 2);
        }

        public void RunTest(string fileName, int mode)
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
                if (mode == 1)
                {
                    SudokuSolver1 solver1 = new SudokuSolver1();
                    solver1.SolveSudoku(test1, n);
                    write += i + " " + solver1.steps + "    " + solver1.solveTime + "    " + solver1.solveTicks + Environment.NewLine;
                }
                if (mode == 2)
                {
                    SudokuSolver2 solver = new SudokuSolver2();
                    solver.SolveSudoku(test1, n);
                    write += i + " " + solver.steps + "    " + solver.solveTime + "    " + solver.solveTicks + Environment.NewLine;
                }
                i++;
                if (i == 35) break;

                Console.WriteLine(@"Sudoku {0} solved", i - 1);
            }
            string loc = "..\\..\\puzzles\\out" + mode + "_" + fileName + ".txt";
            System.IO.File.WriteAllText(loc, write);
            Console.WriteLine(@"Output saved to file: " + loc);
        }
    }

    public static class Parser
    {
        public static List<int [,]> Parser_p096(string [] input)
        {
            List<int[,]> output = new List<int[,]>();
            bool newPuzzle = true;
            int n_puzzle = 0;
            int n = 9;
            while (newPuzzle)
            {
                try
                {
                    var temp = input[10 * (n_puzzle + 1)];
                }
                catch
                {
                    newPuzzle = false;
                }
                int[,] puzzle = new int[n, n];
                int i = 0;
                for( int line = 10* n_puzzle + 1; line < 10*(n_puzzle + 1); line++)
                {
                    var temp = new[] { input[line] };
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
                string inp = input[2 * n_puzzle];
                inp = inp.Substring(0, n * n);
                int i = 0;
                for(int x = 0; x < n; x++)
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