using System.Collections.Generic;

namespace LinearProgramming.Parsing
{
    internal class ConstraintParser
    {
        private readonly TermParser _termParser;

        public ConstraintParser(TermParser termParser)
        {
            _termParser = termParser;
        }

        public Constraint ParseConstraint(string aConstraint)
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

            foreach (var term in terms)
            {
                coeffList.Add(_termParser.ParseTerm(term));
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
    }
}
