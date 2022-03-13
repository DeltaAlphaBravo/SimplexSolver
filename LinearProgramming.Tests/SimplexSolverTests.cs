using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using LinearProgramming.Parsing;
using System.Linq;

namespace LinearProgramming.Tests
{
    [TestClass]
    public class SimplexSolverTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.ComparisonType.LESSTHAN, new List<Term> { 
                new Term { Coefficient = 1.0, VariableSubscript = 1 }, 
                new Term { Coefficient = 1.0, VariableSubscript = 2 } 
            });
            constraints[1] = new Constraint(1, Constraint.ComparisonType.LESSTHAN, new List<Term> {
                new Term { Coefficient = 1.0, VariableSubscript = 1 },
                new Term { Coefficient = -1.0, VariableSubscript = 2 }
            });

            var objective = new ObjectiveFunction()
            {
                Terms = new[] 
                {
                    new Term { Coefficient = 3.0, VariableSubscript = 1 },
                    new Term { Coefficient = 1.0, VariableSubscript = 2 } 
                },
                Optimization = Optimization.Maximize
            };

            var solution = new SimplexSolver().GetSolution(new LinearProgram(objective, constraints));

            solution.SolutionValues.Should().HaveCount(2);

            solution.SolutionValues.Should().OnlyContain(t => t.VariableSubscript == 1 && t.Coefficient == 1.0
                                                           || t.VariableSubscript == 2 && t.Coefficient == 0.0);

            solution.OptimalValue.Should().Be(3);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.ComparisonType.GREATERTHAN, new List<Term> {
                new Term { Coefficient = 1.0, VariableSubscript = 1 },
                new Term { Coefficient = 1.0, VariableSubscript = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.ComparisonType.LESSTHAN, new List<Term> {
                new Term { Coefficient = 1.0, VariableSubscript = 1 },
                new Term { Coefficient = 1.0, VariableSubscript = 2 }
            });
            var objective = new ObjectiveFunction()
            {
                Terms = new[]
    {
                    new Term { Coefficient = 3.0, VariableSubscript = 1 },
                    new Term { Coefficient = 1.0, VariableSubscript = 2 }
                },
                Optimization = Optimization.Maximize
            };

            ((Action)(() => new SimplexSolver().GetSolution(new LinearProgram(objective, constraints)))).Should().Throw<Exception>();
        }

        [TestMethod]
        public void TestMethod3()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.ComparisonType.LESSTHAN, new List<Term> {
                new Term { Coefficient = 1.0, VariableSubscript = 1 },
                new Term { Coefficient = 2.0, VariableSubscript = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.ComparisonType.GREATERTHAN, new List<Term> {
                new Term { Coefficient = 0.0, VariableSubscript = 1 },
                new Term { Coefficient = 1.0, VariableSubscript = 2 }
            });
            var objective = new ObjectiveFunction()
            {
                Terms = new[]
    {
                    new Term { Coefficient = 1.0, VariableSubscript = 1 },
                    new Term { Coefficient = 3.0, VariableSubscript = 2 }
                },
                Optimization = Optimization.Maximize
            };

            var solution = new SimplexSolver().GetSolution(new LinearProgram(objective, constraints));

            solution.SolutionValues.Should().OnlyContain(t => t.VariableSubscript == 1 && t.Coefficient == 0.0
                                                           || t.VariableSubscript == 2 && t.Coefficient == 0.5);
            solution.OptimalValue.Should().Be(1.5);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.ComparisonType.GREATERTHAN, new List<Term> {
                new Term { Coefficient = 1.0, VariableSubscript = 1 },
                new Term { Coefficient = 1.0, VariableSubscript = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.ComparisonType.GREATERTHAN, new List<Term> {
                new Term { Coefficient = 0.0, VariableSubscript = 1 },
                new Term { Coefficient = 1.0, VariableSubscript = 2 }
            });
            var objective = new ObjectiveFunction()
            {
                Terms = new[]
    {
                    new Term { Coefficient = 1.0, VariableSubscript = 1 },
                    new Term { Coefficient = 3.0, VariableSubscript = 2 }
                },
                Optimization = Optimization.Maximize
            };

            var solution = new SimplexSolver().GetSolution(new LinearProgram(objective, constraints));

            solution.SolutionValues.Should().BeNull();
            solution.OptimalValue.Should().BeNull();
        }

        [TestMethod]
        public void TestMethod5()
        {
            var parser = StandardFormLinearProgramParser.CreateDefault();
            var program = parser.Parse(@"Maximize: 1.0x1 + 3.0x2 
                                         Subject to: 1.0x1 + 1.0x2 >= 1
                                                     0.0x1 + 1.0x2 >= 0");

            var solution = new SimplexSolver().GetSolution(program);

            solution.SolutionValues.Should().BeNull();
            solution.OptimalValue.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(@"Minimize: 4x1 + 5x2 + 6x3
                   Subject to: 1x1 + 1x2 >= 11
                               1x1 + -1x2 <= 5
                               1x3 + -1x1 + -1x2 = 0
                               7x1 + 12x2 >= 35
                               1x1 >= 0
                               1x2 >= 0 
                               1x3 >= 0",
            113, 
            8, 3, 11)]
        [DataRow(@"Maximize: 5x1 + 6x2
                   Subject to: 1x1 + 1x2 <= 10
                               1x1 + -1x2 >= 3
                               5x1 + 4x2 <= 35
                               1x1 >= 0
                               1x2 >= 0",
            39.444,
            5.222, 2.222)]
        [DataRow(@"Maximize: 1x3+1x4
                   Subject to: 1x1+-1x3 = 1
                               3x2+-1x4 = 1
                               1x1+1x2 <= 3
                               1x1 <= 3
                               1x2 <= 2
                               1x1 >= 0
                               1x2 >= 0",
            5,
            1, 2, 0, 5)]
        public void TestMethod6(string programString, double optimalValue, params double[] coefficients)
        {
            var parser = StandardFormLinearProgramParser.CreateDefault();
            var program = parser.Parse(programString);

            var solution = new SimplexSolver().GetSolution(program);

            solution.OptimalValue.Should().BeApproximately(optimalValue, 0.0005);
            foreach(var solutionValue in solution.SolutionValues)
            {
                var expected = coefficients[solutionValue.VariableSubscript - 1];
                solutionValue.Coefficient.Should().BeApproximately(expected, 0.0005);
            }
            solution.SolutionValues.Count().Should().Be(coefficients.Length);
        }
    }
}
