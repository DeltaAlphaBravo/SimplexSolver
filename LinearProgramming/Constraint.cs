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

        public Constraint(double aRHS, int aComparison, List<double[]> aCoeffs)
        {
            _rightHandSide = aRHS;

            _sign = aComparison;
            _coefficients = new double[aCoeffs.Count,2];
            for (int x = 0; x < aCoeffs.Count; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    _coefficients[x,y] = (aCoeffs[x])[y];
                }
            }
        }

        public double getTheRHS()
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
