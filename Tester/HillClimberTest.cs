using System;
using ConsoleApplication4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tester
{
    [TestClass]
    public class HillClimberTest
    {
        [TestMethod]
        public void Test_CheckConflicts()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku testSudoku = new HillSudoku(input[0], 9);

            Random rand = new Random(1);

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //CheckConflicts:
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0,0)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 1)));
            Assert.IsFalse(climber.state.CheckConflicts(new Cell(0, 2)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 3)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 4)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 5)));
            Assert.IsFalse(climber.state.CheckConflicts(new Cell(0, 6)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 7)));
            Assert.IsTrue(climber.state.CheckConflicts(new Cell(0, 8)));
        }

        [TestMethod]
        public void Test_FitnessImproved()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku testSudoku = new HillSudoku(input[0], 9);

            Random rand = new Random(1);

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //Check if Fitness is better:
            for (int i = 0; i < 1000; i++)
            {
                climber.random = rand;
                climber.state.ResetInstant(rand);
                int old = climber.state.TotalFitness();
                climber.HillClimb();
                int climbed = climber.state.TotalFitness();
                Assert.IsTrue(old <= climbed);
                //Console.WriteLine("old: {0}, new: {1}", old, climbed);
            }
        }

        [TestMethod]
        public void Test_RandomRestart()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku testSudoku = new HillSudoku(input[0], 9);

            Random rand = new Random();

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //Check Random Restart:
            climber.RandomRestartHillClimb(rand, true, 10000);
            Console.WriteLine("total fitness: {0}", climber.state.TotalFitness());
            Console.WriteLine("total restarts: {0}", climber.restarts);
        }

        [TestMethod]
        public void Test_ILSnonA_adaptive()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku testSudoku = new HillSudoku(input[0], 9);

            Random rand = new Random();

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //Check Random Restart:
            climber.IteratedLocalSearch(rand, 10, true);
            Console.WriteLine("total fitness: {0}", climber.state.TotalFitness());
            Console.WriteLine("total restarts: {0}", climber.restarts);
        }

        [TestMethod]
        public void RefTest()
        {
            int[,] a1 = new int[9, 9];
            int[,] a2 = new int[9, 9];
            Random radn = new Random(1);

            for(int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int r = radn.Next();
                    a1[i, j] = r;
                    a2[i, j] = r;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Assert.AreEqual(a1[i, j], a2[i, j]); ;
                }
            }

            Assert.IsTrue(a2.Equals(a2));
            Assert.IsTrue(a1.Equals(a1));
            Assert.IsTrue(!a1.Equals(a2));
            
        }

        [TestMethod]
        public void TestTabu()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\p096_sudoku.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            int tabuSize = 5;
            foreach (var x in input)
            {
                HillSudoku problem = new HillSudoku(input[0], 9);
                TabuSearch solver = new TabuSearch(problem, tabuSize);
                Console.WriteLine("Testing TabuSearch");
                solver.Search();
                problem.PrintState();
                Console.WriteLine("Ticks: " + solver.solveTicks);
                Console.WriteLine("Steps: " + solver.steps);
                if(problem.CheckSudoku())
                {
                    Console.WriteLine("CORRECTLY SOLVED");
                }
                else
                {
                    Console.WriteLine("Incorrect");
                }
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            string file = "..\\..\\..\\ConsoleApplication4\\puzzles\\1.txt";
            string[] read = System.IO.File.ReadAllLines(file);
            var input = Parser.Parser_p096(read);
            HillSudoku testSudoku = new HillSudoku(input[0], 9);

            Random rand = new Random(1);

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //Basic case:
            Console.WriteLine("RANDOM:");
            climber.state.PrintState();
            Console.WriteLine("total fitness: {0}", climber.state.TotalFitness());

            climber.HillClimb();

            Console.WriteLine("CLIMBED:");
            climber.state.PrintState();
            Console.WriteLine("total fitness: {0}", climber.state.TotalFitness());

            Console.WriteLine();

            //Check Random Restart:
            climber.RandomRestartHillClimb(rand);
            climber.state.PrintState();
            Console.WriteLine("total fitness: {0}", climber.state.TotalFitness());
            Console.WriteLine("total restarts: {0}", climber.restarts);
        }
    }
}
