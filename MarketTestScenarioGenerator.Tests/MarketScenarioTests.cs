using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MarketTestScenarioGenerator.Tests
{
    [TestClass]
    public class MarketScenarioTests
    {
        [TestMethod]
        public void Test1()
        {
            var buyers = new[] { new Buyer("A", 3, 5), new Buyer("B", 2, 7) };
            var sellers = new[] { new Seller("C", 3, 4), new Seller("D", 4, 9) };

            var target = new MarketScenario(buyers, sellers);

            var actual = target.ToLinearProgram();

            actual.Objective.Optimization.Should().Be(LinearProgramming.Optimization.Maximize);
            actual.Objective.Terms.Should().OnlyContain(x => x.Coefficient == -1 && (x.VariableSubscript == 1 || x.VariableSubscript == 3));

            actual.Constraints.Should().Contain(x => x.RightHandSide == 1 && x.Sign == LinearProgramming.Constraint.ComparisonType.EQUALTO && x.Terms.Any(x => x.Coefficient == 7 - 4));
        }
    }
}
