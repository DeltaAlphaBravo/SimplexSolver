using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearProgramming
{
    public class Constraint
    {
        public double RightHandSide { get; private set; }

        public ComparisonType Sign { get; private set; }

        public Term[] Terms { get; private set; }

        public Constraint(double aRHS, ComparisonType aComparison, IEnumerable<Term> terms)
        {
            RightHandSide = aRHS;
            Sign = aComparison;
            Terms = terms.ToArray();
        }

        public static ComparisonType OppositeSign(ComparisonType aSign)
        {
            return  (ComparisonType)((int)aSign * (-1));
        }

        public enum ComparisonType
        {
            GREATERTHAN = 1,
            LESSTHAN = -1,
            EQUALTO = 0
        }
    }
}
