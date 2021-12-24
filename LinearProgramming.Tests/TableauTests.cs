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
            constraints[0] = new Constraint(1, Constraint.LESSTHAN, new List<ConstraintTerm> { 
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 }, 
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 } 
            });
            constraints[1] = new Constraint(1, Constraint.LESSTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 },
                new ConstraintTerm { Coefficient = -1.0, Variable = 2 }
            });

            var target = Tableau.Create(constraints, new[,] { { 3.0, 1.0 }, { 1.0, 2.0 } }, 2);

            target.Solve(Tableau.MAX).Should().BeTrue();

            target.GetSolution().Should().Be(3);

            target.GetSolutionValues().Should().HaveCount(3);

            target.GetSolutionValues().Should().ContainInOrder(1.0, 0.0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.GREATERTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.LESSTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 }
            });

            ((Action)(() => Tableau.Create(constraints, new[,] { { 3.0, 1.0 }, { 1.0, 2.0 } }, 2))).Should().Throw<Exception>();
        }

        [TestMethod]
        public void TestMethod3()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.LESSTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 2.0, Variable = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.GREATERTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 0.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 }
            });

            var target = Tableau.Create(constraints, new[,] { { 1.0, 1.0 }, { 3.0, 2.0 } }, 2);

            target.Solve(Tableau.MAX).Should().BeTrue();

            target.GetSolution().Should().Be(1.5);

            target.GetSolutionValues().Should().ContainInOrder(0.0, 0.5);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var constraints = new Constraint[2];
            constraints[0] = new Constraint(1, Constraint.GREATERTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 1.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 }
            });
            constraints[1] = new Constraint(0, Constraint.GREATERTHAN, new List<ConstraintTerm> {
                new ConstraintTerm { Coefficient = 0.0, Variable = 1 },
                new ConstraintTerm { Coefficient = 1.0, Variable = 2 }
            });

            var target = Tableau.Create(constraints, new[,] { { 1.0, 1.0 }, { 3.0, 2.0 } }, 2);

            target.Solve(Tableau.MAX).Should().BeFalse();
        }
    }
}
