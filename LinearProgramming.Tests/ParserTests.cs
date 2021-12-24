using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinearProgramming.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            LookupTable table = new LookupTable();
            Constraint[] dmitri = Parser.parseAllConstraints("1x1 + 1x2 + 1x3 = 60 \n 1x1 + 1x2 >= 45 \n 5x1 + -6x2 <=10 ", table);
            //      Constraint[] dmitri = parseAllConstraints("1x1+3x2+2x3<=20\n1x1+1x2+1x3<=10", table);
            String george = "3x1 + 2x2 + 4x3";
            try
            {
                Tableau martha = Tableau.Create(dmitri, Parser.parseObjective(george, table), table.getSize());

                martha.Solve(Tableau.MIN);
                double[] bob = martha.GetSolutionValues();
                for (int i = 0; i < bob.Length; i++)
                {
                    Console.WriteLine(table.reverseLookup(i) + " " + bob[i]);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
