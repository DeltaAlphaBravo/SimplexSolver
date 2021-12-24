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

            List<ConstraintTerm> coeffList = new();

            double rhs;
            int comparison = -1;
            int comparisonType = 0;

            int left = 0;
            int right = -1;
            if (aConstraint.IndexOf(">=") != -1)
            {
                comparison = aConstraint.IndexOf(">=") - 1;
                comparisonType = Constraint.GREATERTHAN;
            }
            else if (aConstraint.IndexOf("<=") != -1)
            {
                comparison = aConstraint.IndexOf("<=") - 1;
                comparisonType = Constraint.LESSTHAN;
            }
            else if (aConstraint.IndexOf("=") != -1)
            {
                comparison = aConstraint.IndexOf("=") - 1;
                comparisonType = Constraint.EQUALTO;
            }

            while (right < comparison)
            {
                left = right + 1;
                right = aConstraint.IndexOf('x', Math.Max(right, 0));
                double coeff = Double.Parse(aConstraint.Substring(left, right));
                left = right + 1; //skip over the x
                right = aConstraint.IndexOf('+', right);
                if (right == -1)
                {
                    right = comparison + 1;
                }
                int var = aTable.lookup(int.Parse(aConstraint.Substring(left, right)));

                Console.WriteLine("Constraint: " + coeff + " " + var);

                ConstraintTerm pair = new() { Coefficient = coeff, Variable = var };
                coeffList.Add(pair);
            }

            if (comparisonType == Constraint.EQUALTO)
            {
                rhs = Double.Parse(aConstraint.Substring(comparison + 2,
                                                            aConstraint.Length));
            }
            else
            {
                rhs = Double.Parse(aConstraint.Substring(comparison + 3,
                      aConstraint.Length));
            }

            //rhs must be positive
            if (rhs < 0)
            {
                rhs *= -1;
                comparisonType = Constraint.oppositeSign(comparisonType);
                for (int i = 0; i < coeffList.Count; i++)
                {
                    coeffList[i] = new() { Coefficient = coeffList[i].Coefficient * (-1), Variable = coeffList[i].Variable };
                }
            }

            return new Constraint(rhs, comparisonType, coeffList);
        }

        public static double[,] parseObjective(String anObjective, LookupTable aTable)
        {
            anObjective = anObjective.Replace("\\s", ""); //clean the whitespace out.

            List<double[]> coeffList = new List<double[]>();

            int left = 0;
            int right = -1;

            while (right < anObjective.Length)
            {
                left = right + 1;
                right = anObjective.IndexOf('x', right);
                double coeff = Double.Parse(anObjective.Substring(left, right));
                left = right + 1; //skip over the x
                right = anObjective.IndexOf('+', right);
                if (right == -1)
                {
                    right = anObjective.Length;
                }
                int var = aTable.lookup(int.Parse(anObjective.Substring(left, right)));
                double[] pair = { coeff, var };

                Console.WriteLine("Objective: " + coeff + " " + var);

                coeffList.Add(pair);
            }

            double[,] coeffs = new double[coeffList.Count, 2];
            for (int x = 0; x < coeffList.Count; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    coeffs[x,y] = coeffList[x][y];
                }
            }

            return coeffs;
        }

        public static Constraint[] parseAllConstraints(String anLP, LookupTable aTable)
        {
            List<Constraint> result = new List<Constraint>();

            int left = 0;
            int right = 0;

            while (left < anLP.Length)
            {
                right = anLP.IndexOf('\n', left);
                if (right != -1)
                {
                    result.Add(parseConstraint(anLP.Substring(left, right), aTable));
                    left = right + 1;
                }
                else
                {
                    result.Add(parseConstraint(anLP.Substring(left), aTable));
                    left = anLP.Length;
                }
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
