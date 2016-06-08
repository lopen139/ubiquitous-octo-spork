using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sudoku Set 1
            //Console.WriteLine("<>-------- Solving p096_sudoku with method 1 --------<>");
            //RunTest("p096_sudoku", 1);
            //Console.WriteLine("<>-------- Solving p096_sudoku with method 2 --------<>");
            //RunTest("p096_sudoku", 2);
            //Console.WriteLine("<>-------- Solving p096_sudoku with method 3 --------<>");
            //RunTest("p096_sudoku", 3);
            //Console.WriteLine("<>-------- Solving p096_sudoku with method 4 --------<>");
            //RunTest("p096_sudoku", 4);
            //Sudoku Set 2: hard
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 1 --------<>");
            //RunTest("su17ExtremeDiff500", 1);
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 2 --------<>");
            //RunTest("su17ExtremeDiff500", 2);
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 3 --------<>");
            //RunTest("su17ExtremeDiff500", 3);
            //Console.WriteLine("<>-------- Solving su17ExtremeDiff500 with method 4 --------<>");
            //RunTest("su17ExtremeDiff500", 4);

            Analyzer();

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
                if (mode == 3)
                {
                    HillSudoku sudoku = new HillSudoku(test1, n);
                    HillClimber solver = new HillClimber(sudoku);
                    solver.RandomRestartHillClimb(new Random());
                    write += i + " " + solver.restarts + "    " + solver.solveTime + "    " + solver.solveTicks +
                             Environment.NewLine;
                    solver.state.PrintState();
                }
                if (mode == 4)
                {
                    HillSudoku sudoku = new HillSudoku(test1, n);
                    HillClimber solver = new HillClimber(sudoku);
                    solver.IteratedLocalSearch(new Random(), 20, true);
                    write += i + " " + solver.restarts + "    " + solver.solveTime + "    " + solver.solveTicks +
                             Environment.NewLine;
                    solver.state.PrintState();
                }
                i++;
            }
            string loc = "..\\..\\puzzles\\out" + mode + "_" + fileName + ".txt";
            System.IO.File.WriteAllText(loc, write);
            Console.WriteLine(@"Output saved to file: " + loc);
        }

        /// <summary>
        /// Calculates standard deviation from a list of longs
        /// </summary>
        /// <param name="list"></param>
        /// <returns>standard deviation of list</returns>
        public static double StandardDeviation(List<long> list)
        {
            double avg = list.Average();
            return Math.Sqrt(list.Average(v => Math.Pow(v - avg, 2)));
        }
        public static double StandardDeviation(List<double> list)
        {
            double avg = list.Average();
            return Math.Sqrt(list.Average(v => Math.Pow(v - avg, 2)));
        }

        /// <summary>
        /// Makes analysis file
        /// </summary>
        public static void Analyzer()
        {
            string fileName = "p096_sudoku";
            var input = System.IO.File.ReadAllLines("..\\..\\puzzles\\" + fileName + ".txt");
            List<int[,]> parsed = Parser.Parser_p096(input);
            int i = 0;
            int n = 9;
            string str =
                "i averageRestarts sdRestarts averageSolveTicks sdSolveTicks averageAverageSteps sdAverageSteps averageAverageTicks sdAverageTicks averageAverageMilliseconds sdAverageMilliseconds";
            Console.WriteLine(str);
            int solves = 100;
            Console.WriteLine("solves: " + solves);
            string write = str + Environment.NewLine;
            write += solves + Environment.NewLine;
            foreach (var test1 in parsed)
            {
                {
                    HillSudoku sudoku = new HillSudoku(test1, n);
                    HillClimber solver = new HillClimber(sudoku);

                    //Analysis parameters
                    List<long> restarts = new List<long>();
                    List<long> solveTicks = new List<long>();
                    List<long> solveTMilliseconds = new List<long>();
                    List<double> averageSteps = new List<double>();
                    List<double> averageTicks= new List<double>();
                    List<double> averageMilliseconds = new List<double>();
                    //List<double> sdSteps = new List<double>();
                    //List<double> sdTicks = new List<double>();
                    //<double> sdMilliseconds = new List<double>();

                    //Multiple solves for each sudoku for more statistics
                    for (int j = 0; j < solves; j++)
                    {
                        solver.RandomRestartHillClimb(new Random());

                        //Document analysis parameters
                        restarts.Add(solver.restarts);
                        solveTicks.Add(solver.solveTicks);
                        solveTMilliseconds.Add(solver.solveTime);
                        averageSteps.Add(solver.averageSteps);
                        //sdSteps.Add(solver.sdSteps);
                        averageTicks.Add(solver.averageTicks);
                        averageMilliseconds.Add(solver.averageMilliseconds);
                        //sdTicks.Add(solver.sdTicks);
                        //sdMilliseconds.Add(solver.sdMilliseconds);

                        //Reset analysis parameters in the solver
                        solver.ResetAnalysisParameters();
                    }
                    //Process statistics per Sudoku
                    //restarts:
                    double averageRestarts = restarts.Average();
                    double sdRestarts = StandardDeviation(restarts);
                    //solve ticks:
                    double averageSolveTicks = solveTicks.Average();
                    double sdSolveTicks = StandardDeviation(solveTicks);
                    //average steps:
                    double averageAverageSteps = averageSteps.Average();
                    double sdAverageSteps = StandardDeviation(averageSteps);
                    //double averageSDSteps = sdSteps.Average();
                    //average ticks:
                    double averageAverageTicks = averageTicks.Average();
                    double sdAverageTicks = StandardDeviation(averageTicks);
                    //double averageSDTicks = sdTicks.Average();
                    //average ms:
                    double averageAverageMilliseconds = averageMilliseconds.Average();
                    double sdAverageMilliseconds = StandardDeviation(averageMilliseconds);
                    //double averageSDMilliseconds = sdMilliseconds.Average();

                    //Write Line to file
                    string line = i + " " + averageRestarts + "    " + sdRestarts + "    " + averageSolveTicks + "  " + sdSolveTicks + "    " + averageAverageSteps + 
                        "   " + sdAverageSteps + "  " + averageAverageTicks + " " + sdAverageTicks + "  " + averageAverageMilliseconds + 
                        "   " + sdAverageMilliseconds;
                    Console.WriteLine(line);
                    write += line + Environment.NewLine;
                    solver.state.PrintState();
                }
                /*
                if (mode == 4)
                {
                    HillSudoku sudoku = new HillSudoku(test1, n);
                    HillClimber solver = new HillClimber(sudoku);
                    solver.IteratedLocalSearch(new Random(), 20);
                    write += i + " " + solver.restarts + "    " + solver.solveTime + "    " + solver.solveTicks +
                             Environment.NewLine;
                    solver.state.PrintState();
                }
                */
                i++;
            }
            string loc = "..\\..\\puzzles\\out" + "_analysis_simplesudoku" + "_" + fileName + ".txt";
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