using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using LinearProgramming.Parsing;

namespace LinearProgramming.Tests
{
    [TestClass]
    public class StandardFormLinearProgramParserTests
    {
        [TestMethod]
        public void TestMaximization()
        {
            var target = StandardFormLinearProgramParser.CreateDefault();
            var actual = target.Parse(@"Maximize: 1.0x1 + -2x2 
                                        Subject to: 1x1 + 1x2 + 1x3 = 60
                                                    1x1 + 1x2 >= 45
                                                    5x1 + -6x2 <=10 ");

            actual.Objective.Optimization.Should().Be(Optimization.Maximize);
            actual.Objective.Terms[0].Coefficient.Should().Be(1.0);
            actual.Objective.Terms[0].VariableSubscript.Should().Be(1);
            actual.Objective.Terms[1].Coefficient.Should().Be(-2);
            actual.Objective.Terms[1].VariableSubscript.Should().Be(2);

            actual.Constraints.Length.Should().Be(3);

            actual.Constraints[0].Terms[0].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[0].Terms[1].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[0].Terms[2].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[2].VariableSubscript.Should().Be(3);
            actual.Constraints[0].Sign.Should().Be(Constraint.ComparisonType.EQUALTO);
            actual.Constraints[0].RightHandSide.Should().Be(60);

            actual.Constraints[1].Terms[0].Coefficient.Should().Be(1);
            actual.Constraints[1].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[1].Terms[1].Coefficient.Should().Be(1);
            actual.Constraints[1].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[1].Sign.Should().Be(Constraint.ComparisonType.GREATERTHAN);
            actual.Constraints[1].RightHandSide.Should().Be(45);

            actual.Constraints[2].Terms[0].Coefficient.Should().Be(5);
            actual.Constraints[2].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[2].Terms[1].Coefficient.Should().Be(-6);
            actual.Constraints[2].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[2].Sign.Should().Be(Constraint.ComparisonType.LESSTHAN);
            actual.Constraints[2].RightHandSide.Should().Be(10);
        }

        [TestMethod]
        public void TestMinimization()
        {
            var target = StandardFormLinearProgramParser.CreateDefault();
            var actual = target.Parse(@"Minimize: 1.0x1 + -2x2 
                                        Subject to: 1x1 + 1x2 + 1x3 = 60
                                                    1x1 + 1x2 >= 45
                                                    5.0x1 + -6x2 <=10 ");

            actual.Objective.Optimization.Should().Be(Optimization.Minimize);
            actual.Objective.Terms[0].Coefficient.Should().Be(1.0);
            actual.Objective.Terms[0].VariableSubscript.Should().Be(1);
            actual.Objective.Terms[1].Coefficient.Should().Be(-2);
            actual.Objective.Terms[1].VariableSubscript.Should().Be(2);

            actual.Constraints.Length.Should().Be(3);

            actual.Constraints[0].Terms[0].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[0].Terms[1].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[0].Terms[2].Coefficient.Should().Be(1);
            actual.Constraints[0].Terms[2].VariableSubscript.Should().Be(3);
            actual.Constraints[0].Sign.Should().Be(Constraint.ComparisonType.EQUALTO);
            actual.Constraints[0].RightHandSide.Should().Be(60);

            actual.Constraints[1].Terms[0].Coefficient.Should().Be(1);
            actual.Constraints[1].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[1].Terms[1].Coefficient.Should().Be(1);
            actual.Constraints[1].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[1].Sign.Should().Be(Constraint.ComparisonType.GREATERTHAN);
            actual.Constraints[1].RightHandSide.Should().Be(45);

            actual.Constraints[2].Terms[0].Coefficient.Should().Be(5);
            actual.Constraints[2].Terms[0].VariableSubscript.Should().Be(1);
            actual.Constraints[2].Terms[1].Coefficient.Should().Be(-6);
            actual.Constraints[2].Terms[1].VariableSubscript.Should().Be(2);
            actual.Constraints[2].Sign.Should().Be(Constraint.ComparisonType.LESSTHAN);
            actual.Constraints[2].RightHandSide.Should().Be(10);
        }
    }
}
