using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ConsoleApplication4;

namespace Tester
{
    [TestClass]
    public class Analyzer
    {
        [TestMethod]
        public void Test1()
        {
            var input = System.IO.File.ReadAllLines("..\\..\\puzzles\\p096_sudoku.txt");
            var parsed = Parser.Parser_p096(input);
            int i = 0;
            int n = 9;
            foreach (var test in parsed)
            {
                Console.WriteLine("SUDOKU #{0}:" , i);
                Console.WriteLine("unsolved");
                SudokuSolver solver = new SudokuSolver();
                solver.SolveSudoku(test, n);
                Console.WriteLine("solved");
                solver.sudoku.PrintSudoku();
                i++;
            }
        }
        [TestMethod]
        public void Test2()
        {
            var input = System.IO.File.ReadAllLines("..\\..\\puzzles\\su17ExtremeDiff500.txt");
            var parsed = Parser.Parser_su17(input);
            int i = 0;
            int n = 9;
            foreach (var test in parsed)
            {
                Console.WriteLine("SUDOKU #{0}:", i);
                Console.WriteLine("unsolved");
                SudokuSolver solver = new SudokuSolver();
                solver.SolveSudoku(test, n);
                Console.WriteLine("solved");
                solver.sudoku.PrintSudoku();
                i++;
                return;
            }
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
