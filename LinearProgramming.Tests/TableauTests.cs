using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace LinearProgramming.Tests
{
    [TestClass]
    public class TableauTests
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

            //target.Solve(Tableau.MAX).Should().BeFalse();

            solution.SolutionValues.Should().BeNull();
            solution.OptimalValue.Should().BeNull();
        }
    }
}
