using LinearProgramming;
using System;
using System.Linq;

namespace MarketTestScenarioGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var matrix = @"1 2 3 4
                             5 6 7
                               8 9
                                10";
            var parsedMatrix = ParseAdjacencyMatrix(matrix);
            ConsoleWriteMatrix(parsedMatrix);

            var buyers = new[] { new Buyer("A", 3, 5), new Buyer("B", 2, 7), new Buyer("C", 7, 1)};
            var sellers = new[] { new Seller("ZZ", 3, 4), new Seller("YY", 4, 2) };

            MarketScenario marketScenario = new(buyers, sellers);
            var program = marketScenario.ToLinearProgram();

            Display(program);

            try
            {
                var solution = new SimplexSolver().GetSolution(program);
                Console.WriteLine(solution.OptimalValue);
                foreach (var solutionValue in solution.SolutionValues)
                    Console.WriteLine($"{solutionValue.Coefficient} {solutionValue.VariableSubscript}");
                var sales = marketScenario.ReadSolution(solution);
                foreach (var sale in sales)
                    Console.WriteLine($"{sale.QuantitySold} sold from {sale.Seller.Name} to {sale.Buyer.Name}");
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        private static void ConsoleWriteMatrix<T>(T[,] parsedMatrix)
        {
            for (var i = 0; i < parsedMatrix.GetLongLength(0); i++)
            {
                for (var j = 0; j < parsedMatrix.GetLongLength(1); j++)
                    Console.Write($"{parsedMatrix[i, j]} \t");
                Console.WriteLine();
            }
        }

        static double[,] ParseAdjacencyMatrix(string matrix)
        {
            var lines = matrix.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var cells = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var size = cells.Length + 1;
            var result = new double[size, size];
            var origin = 0;
            var destination = 0;
            do
            {
                cells = lines[origin].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                do
                {
                    var distance = double.Parse(cells[destination - origin]);
                    result[origin, destination + 1] = distance;
                    result[destination + 1, origin] = distance;
                    destination++;
                } while (destination + 1 < size);
                destination = ++origin;
            } while (origin + 1 < size);
            return result;
        }

        static void Display(LinearProgram program)
        {
            var optimizationTerms = program.Objective.Terms.Select(t => $"{t.Coefficient}x{t.VariableSubscript}");
            Console.WriteLine($"{program.Objective.Optimization} {string.Join("+", optimizationTerms)}");
            foreach (var constraint in program.Constraints)
            {
                var constraintTerms = constraint.Terms.Select(t => $"{t.Coefficient}x{t.VariableSubscript}");
                Console.WriteLine($"{string.Join("+", constraintTerms)} {constraint.Sign} {constraint.RightHandSide}");
            }
        }
    }
}
