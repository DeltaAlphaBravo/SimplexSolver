using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming
{
    public class Tableau
    {
        public const bool MIN = true;
        public const bool MAX = false;

        private readonly double[,] _tableau;
        private double[,] _a;
        private double[] _c;
        private double[] _b;
        private double _z;

        private readonly int[] _basis;

        private readonly int _numRows;
        private readonly int _numCols;
        private readonly int _numUserVars;

        public Tableau(double[,] aMatrix, int[] anInitialBasis, int aNumUserVars)
        {
            _tableau = aMatrix;
            _numRows = _tableau.GetLength(0);
            _numCols = _tableau.GetLength(1);
            CutUpTableau();
            _basis = anInitialBasis;
            _numUserVars = aNumUserVars;
        }

        public static Tableau Create(Constraint[] aConstraints, double[,] anObj,
              int aNumUserVars)
        {
            int numVars = aNumUserVars;
            List<int[]> rowsWithArtVars = new List<int[]>();

            int [] basis = new int[1 + aConstraints.Length];
            basis[0] = 0;

            for (int x = 0; x < aConstraints.Length; x++)
            {
                if (aConstraints[x].getTheSign() == Constraint.LESSTHAN)
                {
                    numVars++;
                }
                if (aConstraints[x].getTheSign() == Constraint.GREATERTHAN)
                {
                    numVars += 2;
                }
                if (aConstraints[x].getTheSign() == Constraint.EQUALTO)
                {
                    numVars++;
                }
            }

            var numRows = aConstraints.Length + 1;
            var numCols = numVars + 2;

            var tableau = new double[numRows, numCols];

            tableau[0, 0] = 1;

            int numSlackVars = 0;
            for (int x = 0; x < aConstraints.Length; x++)
            {
                tableau[x + 1, 0] = 0;
                for (int y = 0; y < aConstraints[x].getTheCoeffs().GetLength(0); y++)
                {
                    tableau[x + 1, (int)aConstraints[x].getTheCoeffs()[y, 1]] = aConstraints[x].getTheCoeffs()[y, 0];
                }

                tableau[x + 1, numCols - 1] = aConstraints[x].GetTheRHS();

                if (aConstraints[x].getTheSign() == Constraint.LESSTHAN)
                {
                    numSlackVars++;
                    tableau[x + 1, aNumUserVars + numSlackVars] = 1;
                    basis[x + 1] = aNumUserVars + numSlackVars;
                }
                if (aConstraints[x].getTheSign() == Constraint.GREATERTHAN)
                {
                    numSlackVars++;
                    tableau[0, numCols - 2 - rowsWithArtVars.Count] = -1;
                    tableau[x + 1, aNumUserVars + numSlackVars] = -1;
                    tableau[x + 1, numCols - 2 - rowsWithArtVars.Count] = 1;
                    basis[x + 1] = numCols - 2 - rowsWithArtVars.Count;
                    int[] pivotPoint = new int[2];
                    pivotPoint[0] = x + 1;
                    pivotPoint[1] = numCols - 2 - rowsWithArtVars.Count;
                    rowsWithArtVars.Add(pivotPoint);
                }
                if (aConstraints[x].getTheSign() == Constraint.EQUALTO)
                {
                    tableau[x + 1, numCols - 2 - rowsWithArtVars.Count] = 1;
                    tableau[0, numCols - 2 - rowsWithArtVars.Count] = -1;
                    basis[x + 1] = numCols - 2 - rowsWithArtVars.Count;
                    int[] pivotPoint = new int[2];
                    pivotPoint[0] = x + 1;
                    pivotPoint[1] = numCols - 2 - rowsWithArtVars.Count;
                    rowsWithArtVars.Add(pivotPoint);
                }
            }

            var result = new Tableau(tableau, basis, aNumUserVars);

            while (rowsWithArtVars.Any())
            {
                int[] rowToAdd = rowsWithArtVars.First();
                rowsWithArtVars.Remove(rowToAdd);
                result.Pivot(rowToAdd[0], rowToAdd[1]);
                numVars--;
            }
            Console.WriteLine("Initial Table: \n" + result + "\n");

            result.Solve(Tableau.MIN); //End Phase 1. Begin Phase 2.

            if (tableau[0, numCols - 1] != 0)
            {
                String error = "Infeasible based on the min of art vars not being 0" +
                               "Col: " + numCols + " " + tableau[0, numCols - 1];
                Exception e = new Exception(error);
                throw (e);
            }

            /*
             * If this was done better, the tableau would be in a LinkedList type so that artificial rows
             * could be removed. I didn't do that. So, the question becomes about how to deal with them.
             * They don't really add much computation so I'll just leave them zeroed out.
             */

            for (int x = numVars + 1; x < numCols - 1; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    tableau[y, x] = 0;
                }
            }

            //add costs.
            for (int x = 0; x < anObj.GetLength(0); x++)
            {
                tableau[0, (int)anObj[x, 1]] = (-1) * anObj[x, 0];
            }

            Console.WriteLine("Before pivots\n" + result);
            //the z-row didn't necessarily respect the basis variables. Pivot to sort things out.
            for (int j = 1; j < basis.Length; j++)
            {
                try
                {
                    result.Pivot(j, basis[j]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("PHASE 1 TABLE: \n" + result + "\n");

            return result;
        }

        /**
         * Makes (row, var) equal 1 with a multiplication row operation.
         * @param row which row to reduce
         * @param var which variable from left to right to make equal to 1
         */
        private void ReduceRow(int row, int var)
        {
            //TODO: Check for division by zero and that row > 0
            double scalar = _tableau[row, var];
            if (scalar == 0)
            {
                throw new Exception("Divide By Zero");
            }
            for (int x = 0; x < _numCols; x++)
            {
                _tableau[row, x] = _tableau[row, x] / scalar;
            }
        }

        /**
         * Pivots around the point at (row, var)
         * Note: var is greater than 0.
         * @param row 
         * @param var
         */
        private void Pivot(int row, int var)
        {
            //TODO: Check for division by zero and that row > 0
            try
            {
                ReduceRow(row, var);
            }
            catch (Exception e)
            {
                throw e;
            }
            for (int y = 0; y < _numRows; y++)
            {
                if (y != row)
                {
                    double scalar = (-1) * _tableau[y, var];
                    //horrible kludge to deal with float imprecision
                    scalar = Math.Round(scalar * 10000000) / 10000000.0;
                    for (int column = 0; column < _tableau.GetLength(1); column++)
                    {
                        _tableau[y, column] = _tableau[y, column] + Math.Round(_tableau[row, column] * scalar * 10000000) / 10000000.0;
                        //kludge again
                        _tableau[y, column] = Math.Round(_tableau[y, column] * 10000000) / 10000000.0;
                    }
                }
            }
        }

        private void CutUpTableau()
        {
            _c = new double[_numCols - 2];
            for (int column = 0; column < _numCols - 2; column++)
            {
                _c[column] = _tableau[0, column + 1];
            }

            _a = new double[_numRows - 1, _numCols - 1];
            for (int row = 0; row < _a.GetLength(0); row++)
            {
                for (int column = 0; column < _a.GetLength(1); column++)
                {
                    _a[row, column] = _tableau[row + 1, column];
                }
            }

            _b = new double[_numRows - 1];
            for (int row = 0; row < _b.Length; row++)
            {
                _b[row] = _tableau[row + 1, _numCols - 1];
            }

            _z = _tableau[0, _numCols - 1];
        }

        public double[] GetCosts()
        {
            return _c;
        }

        public double[,] GetVarCoeff()
        {
            return _a;
        }

        public double GetSolution()
        {
            return _z;
        }

        public double[] GetSolutionValues()
        {
            //for each user variable
            //   if the variable is in the basis
            //      while not found
            //         if the element of the variable's column is 1
            //            found
            //            make the array element of that user variable = the right hand side
            //   else
            //      make the array element of that variable = 0
            double[] solutions = new double[_numUserVars + 1];
            for (int i = 0; i <= _numUserVars; i++)
            {
                for (int j = 0; j < _basis.Length; j++)
                {
                    if (i == _basis[j])
                    {
                        bool found = false;
                        int k = 0;
                        while (!found && k < _numRows)
                        {
                            if (_tableau[k, i] == 1)
                            {
                                found = true;
                                solutions[i] = _tableau[k, _numCols - 1];
                            }
                            k++;
                        }
                    }
                }
            }
            return solutions;
        }

        public double[,] GetTableau()
        {
            return _tableau;
        }

        public override string ToString()
        {
            String result = new String("");
            for (int column = 0; column < _numCols; column++)
            {
                result = result + "x" + column + "  ";
            }
            result += "\n";
            for (int row = 0; row < _numRows; row++)
            {
                for (int column = 0; column < _numCols; column++)
                {
                    result = result += _tableau[row, column] + " ";
                }
                result += "\n";
            }
            return result;
        }

        /**
         * @return 0 if optimal, variable number if not.
         */
        private int ChooseEntering(bool isMaximizing)
        {
            int variable = 0;
            int x = 1;
            while (x < _numCols - 1 && variable == 0)
            {
                if (isMaximizing == Tableau.MAX && _tableau[0, x] < 0)
                {
                    variable = x;
                }
                if (isMaximizing == Tableau.MIN && _tableau[0, x] > 0)
                {
                    variable = x;
                }
                x++;
            }
            return variable;
        }

        private int ChooseExiting(int anEntering)
        {
            int variable = 0;
            double bestRatio = double.PositiveInfinity;
            for (int row = 1; row < _numRows; row++)
            {
                if (_tableau[row, anEntering] > 0)
                {
                    double ratio = _tableau[row, _numCols - 1] / _tableau[row, anEntering];
                    if (ratio < bestRatio && ratio >= 0)
                    {
                        bestRatio = ratio;
                        variable = _basis[row];
                    }
                }
            }
            return variable;
        }


        private int ReplaceVarInBasis(int anEntering, int anExiting)
        {
            int rowNum = 0;
            bool isFound = false;
            while (!isFound && rowNum < _basis.Length)
            {
                rowNum++;
                if (_basis[rowNum] == anExiting)
                {
                    isFound = true;
                    _basis[rowNum] = anEntering;
                }
            }
            if (!isFound) { rowNum = 0; }
            return rowNum;
        }


        /**
         * @return true if the problem is bounded, false otherwise
         */
        public bool Solve(bool isMaximizing)
        {
            bool bounded = true;
            int entering = this.ChooseEntering(isMaximizing);
            while (entering != 0)
            {
                int exiting = this.ChooseExiting(entering);
                if (exiting > 0)
                {
                    int row = this.ReplaceVarInBasis(entering, exiting);
                    try
                    {
                        this.Pivot(row, entering);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    entering = this.ChooseEntering(isMaximizing);
                }
                else
                {
                    bounded = false;
                    entering = 0;
                }
                Console.WriteLine(this + "\n");
            }
            CutUpTableau();
            return bounded;
        }
    }
}
