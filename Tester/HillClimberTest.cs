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
            Assert.IsTrue(climber.CheckConflicts(0, 0));
            Assert.IsTrue(climber.CheckConflicts(0, 1));
            Assert.IsFalse(climber.CheckConflicts(0, 2));
            Assert.IsTrue(climber.CheckConflicts(0, 3));
            Assert.IsTrue(climber.CheckConflicts(0, 4));
            Assert.IsTrue(climber.CheckConflicts(0, 5));
            Assert.IsFalse(climber.CheckConflicts(0, 6));
            Assert.IsTrue(climber.CheckConflicts(0, 7));
            Assert.IsTrue(climber.CheckConflicts(0, 8));
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
                climber.ResetInstant();
                int old = climber.TotalFitness();
                climber.HillClimb();
                int climbed = climber.TotalFitness();
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

            Random rand = new Random(1);

            testSudoku.RandomizeSudoku(rand);
            HillClimber climber = new HillClimber(testSudoku);

            //Check Random Restart:
            climber.RandomRestartHillClimb(rand, true, 10000);
            Console.WriteLine("total fitness: {0}", climber.TotalFitness());
            Console.WriteLine("total restarts: {0}", climber.restarts);
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
            climber.state.PrintSudoku();
            Console.WriteLine("total fitness: {0}", climber.TotalFitness());

            climber.HillClimb();

            Console.WriteLine("CLIMBED:");
            climber.state.PrintSudoku();
            Console.WriteLine("total fitness: {0}", climber.TotalFitness());

            Console.WriteLine();

            //Check Random Restart:
            climber.RandomRestartHillClimb(rand);
            climber.state.PrintSudoku();
            Console.WriteLine("total fitness: {0}", climber.TotalFitness());
            Console.WriteLine("total restarts: {0}", climber.restarts);
        }
    }
}
