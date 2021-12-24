using System;
using System.Collections.Generic;
using System.Text;

namespace LinearProgramming
{
    public class Constraint
    {
        public const int GREATERTHAN = 1;
        public const int LESSTHAN = -1;
        public const int EQUALTO = 0;

        private readonly double _rightHandSide;
        private readonly int _sign;
        private readonly double[,] _coefficients;

        public Constraint(double aRHS, int aComparison, List<ConstraintTerm> aCoeffs)
        {
            _rightHandSide = aRHS;

            _sign = aComparison;
            _coefficients = new double[aCoeffs.Count, 2];
            for (int x = 0; x < aCoeffs.Count; x++)
            {

                _coefficients[x, 0] = (aCoeffs[x]).Coefficient;
                _coefficients[x, 1] = (aCoeffs[x]).Variable;
            }
        }

        public double GetTheRHS()
        {
            return _rightHandSide;
        }

        public int getTheSign()
        {
            return _sign;
        }

        public double[,] getTheCoeffs()
        {
            return _coefficients;
        }

        public static int oppositeSign(int aSign)
        {
            return aSign * (-1);
        }
    }
}
