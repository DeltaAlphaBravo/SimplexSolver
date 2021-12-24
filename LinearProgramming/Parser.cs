using System;
using System.Collections.Generic;
using System.Text;

namespace LinearProgramming
{
    public class Parser
    {
        public static Constraint parseConstraint(String aConstraint, LookupTable aTable)
        {
            aConstraint = aConstraint.Replace("\\s", ""); //clean the whitespace out.

            List<Term> coeffList = new();

            double rhs;
            int comparison = -1;
            Constraint.ComparisonType comparisonType = 0;

            if (aConstraint.IndexOf(">=") != -1)
            {
                comparison = aConstraint.IndexOf(">=") - 1;
                comparisonType = Constraint.ComparisonType.GREATERTHAN;
            }
            else if (aConstraint.IndexOf("<=") != -1)
            {
                comparison = aConstraint.IndexOf("<=") - 1;
                comparisonType = Constraint.ComparisonType.LESSTHAN;
            }
            else if (aConstraint.IndexOf("=") != -1)
            {
                comparison = aConstraint.IndexOf("=") - 1;
                comparisonType = Constraint.ComparisonType.EQUALTO;
            }

            var terms = aConstraint[..comparison].Split('+');

            foreach(var term in terms)
            {
                coeffList.Add(ParseTerm(term));
            }

            if (comparisonType == Constraint.ComparisonType.EQUALTO)
            {
                rhs = double.Parse(aConstraint.Substring(comparison + 2));
            }
            else
            {
                rhs = double.Parse(aConstraint.Substring(comparison + 3));
            }

            //rhs must be positive
            if (rhs < 0)
            {
                rhs *= -1;
                comparisonType = Constraint.OppositeSign(comparisonType);
                for (int i = 0; i < coeffList.Count; i++)
                {
                    coeffList[i] = new() { Coefficient = coeffList[i].Coefficient * (-1), VariableSubscript = coeffList[i].VariableSubscript };
                }
            }

            return new Constraint(rhs, comparisonType, coeffList);
        }

        private static Term ParseTerm(string term)
        {
            int xIndex = term.IndexOf('x');
            return new Term
            {
                Coefficient = double.Parse(term[..xIndex]),
                VariableSubscript = int.Parse(term[(xIndex + 1)..]) 
            };
        }

        public static ObjectiveFunction parseObjective(String anObjective, LookupTable aTable)
        {
            anObjective = anObjective.Replace("\\s", ""); //clean the whitespace out.
            List<Term> coeffList = new();
            var terms = anObjective.Split('+');

            foreach (var term in terms)
            {
                coeffList.Add(ParseTerm(term));
            }

            return new ObjectiveFunction() { Terms = coeffList };
        }

        public static Constraint[] parseAllConstraints(String anLP, LookupTable aTable)
        {
            List<Constraint> result = new List<Constraint>();

            foreach (var constraint in anLP.Split('\n'))
            {
                result.Add(parseConstraint(constraint, aTable));
            }
            return result.ToArray();
        }

        public static void main()
        {
            LookupTable table = new LookupTable();
            Constraint[] dmitri = parseAllConstraints("1x1 + 1x2 + 1x3 = 60 \n 1x1 + 1x2 >= 45 \n 5x1 + -6x2 <=10 ", table);
            //      Constraint[] dmitri = parseAllConstraints("1x1+3x2+2x3<=20\n1x1+1x2+1x3<=10", table);
            String george = "3x1 + 2x2 + 4x3";
            try
            {
                Tableau martha = Tableau.Create(dmitri, parseObjective(george, table), table.getSize());

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
