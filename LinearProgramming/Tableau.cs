using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Tableau(double[,] aMatrix, int[] anInitialBasis)
        {
            _tableau = aMatrix;
            _numRows = _tableau.GetLength(0);
            _numCols = _tableau.GetLength(1);
            CutUpTableau();
            _basis = anInitialBasis;
        }

        public Tableau(Constraint[] aConstraints, double[,] anObj,
              int aNumUserVars)
        {
            int numVars = aNumUserVars;
            _numUserVars = aNumUserVars;
            List<int[]> rowsWithArtVars = new List<int[]>();

            _basis = new int[1 + aConstraints.Length];
            _basis[0] = 0;

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

            _numRows = aConstraints.Length + 1;
            _numCols = numVars + 2;

            _tableau = new double[_numRows, _numCols];

            _tableau[0, 0] = 1;

            int numSlackVars = 0;
            for (int x = 0; x < aConstraints.Length; x++)
            {
                _tableau[x + 1, 0] = 0;
                for (int y = 0; y < aConstraints[x].getTheCoeffs().GetLength(0); y++)
                {
                    _tableau[x + 1, (int)aConstraints[x].getTheCoeffs()[y, 1]] = aConstraints[x].getTheCoeffs()[y, 0];
                }

                _tableau[x + 1, _numCols - 1] = aConstraints[x].getTheRHS();

                if (aConstraints[x].getTheSign() == Constraint.LESSTHAN)
                {
                    numSlackVars++;
                    _tableau[x + 1, aNumUserVars + numSlackVars] = 1;
                    _basis[x + 1] = aNumUserVars + numSlackVars;
                }
                if (aConstraints[x].getTheSign() == Constraint.GREATERTHAN)
                {
                    numSlackVars++;
                    _tableau[0, _numCols - 2 - rowsWithArtVars.Count] = -1;
                    _tableau[x + 1, aNumUserVars + numSlackVars] = -1;
                    _tableau[x + 1, _numCols - 2 - rowsWithArtVars.Count] = 1;
                    _basis[x + 1] = _numCols - 2 - rowsWithArtVars.Count;
                    int[] pivotPoint = new int[2];
                    pivotPoint[0] = x + 1;
                    pivotPoint[1] = _numCols - 2 - rowsWithArtVars.Count;
                    rowsWithArtVars.Add(pivotPoint);
                }
                if (aConstraints[x].getTheSign() == Constraint.EQUALTO)
                {
                    _tableau[x + 1, _numCols - 2 - rowsWithArtVars.Count] = 1;
                    _tableau[0, _numCols - 2 - rowsWithArtVars.Count] = -1;
                    _basis[x + 1] = _numCols - 2 - rowsWithArtVars.Count;
                    int[] pivotPoint = new int[2];
                    pivotPoint[0] = x + 1;
                    pivotPoint[1] = _numCols - 2 - rowsWithArtVars.Count;
                    rowsWithArtVars.Add(pivotPoint);
                }
            }

            while (rowsWithArtVars.Any())
            {
                int[] rowToAdd = rowsWithArtVars.First();
                rowsWithArtVars.Remove(rowToAdd);
                this.Pivot(rowToAdd[0], rowToAdd[1]);
                numVars--;
            }
            Console.WriteLine("Initial Table: \n" + this + "\n");

            this.Solve(Tableau.MIN); //End Phase 1. Begin Phase 2.

            if (_tableau[0, _numCols - 1] != 0)
            {
                String error = "Infeasible based on the min of art vars not being 0" +
                               "Col: " + _numCols + " " + _tableau[0, _numCols - 1];
                Exception e = new Exception(error);
                throw (e);
            }

            /*
             * If this was done better, the tableau would be in a LinkedList type so that artificial rows
             * could be removed. I didn't do that. So, the question becomes about how to deal with them.
             * They don't really add much computation so I'll just leave them zeroed out.
             */

            for (int x = numVars + 1; x < _numCols - 1; x++)
            {
                for (int y = 0; y < _numRows; y++)
                {
                    _tableau[y, x] = 0;
                }
            }

            //add costs.
            for (int x = 0; x < anObj.GetLength(0); x++)
            {
                _tableau[0, (int)anObj[x, 1]] = (-1) * anObj[x, 0];
            }

            Console.WriteLine("Before pivots\n" + this);
            //the z-row didn't necessarily respect the basis variables. Pivot to sort things out.
            for (int j = 1; j < _basis.Length; j++)
            {
                try
                {
                    Pivot(j, _basis[j]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("PHASE 1 TABLE: \n" + this + "\n");
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
