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

            var target = Tableau.Create(constraints, objective, 2);

            target.Solve(Tableau.MAX).Should().BeTrue();

            target.GetSolution().Should().Be(3);

            target.GetSolutionValues().Should().HaveCount(3);

            target.GetSolutionValues().Should().ContainInOrder(1.0, 0.0);
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

            ((Action)(() => Tableau.Create(constraints, objective, 2))).Should().Throw<Exception>();
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

            var target = Tableau.Create(constraints, objective, 2);

            target.Solve(Tableau.MAX).Should().BeTrue();

            target.GetSolution().Should().Be(1.5);

            target.GetSolutionValues().Should().ContainInOrder(0.0, 0.5);
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

            var target = Tableau.Create(constraints, objective, 2);

            target.Solve(Tableau.MAX).Should().BeFalse();
        }
    }
}
