using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace LinearProgramming.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            LookupTable table = new();
            Constraint[] actual = Parser.parseAllConstraints("1x1 + 1x2 + 1x3 = 60 \n 1x1 + 1x2 >= 45 \n 5x1 + -6x2 <=10 ", table);

            actual.Length.Should().Be(3);

            actual[0].Terms[0].Coefficient.Should().Be(1);
            actual[0].Terms[0].VariableSubscript.Should().Be(1);
            actual[0].Terms[1].Coefficient.Should().Be(1);
            actual[0].Terms[1].VariableSubscript.Should().Be(2);
            actual[0].Terms[2].Coefficient.Should().Be(1);
            actual[0].Terms[2].VariableSubscript.Should().Be(3);
            actual[0].Sign.Should().Be(Constraint.ComparisonType.EQUALTO);
            actual[0].RightHandSide.Should().Be(60);

            actual[1].Terms[0].Coefficient.Should().Be(1);
            actual[1].Terms[0].VariableSubscript.Should().Be(1);
            actual[1].Terms[1].Coefficient.Should().Be(1);
            actual[1].Terms[1].VariableSubscript.Should().Be(2);
            actual[1].Sign.Should().Be(Constraint.ComparisonType.GREATERTHAN);
            actual[1].RightHandSide.Should().Be(45);

            actual[2].Terms[0].Coefficient.Should().Be(5);
            actual[2].Terms[0].VariableSubscript.Should().Be(1);
            actual[2].Terms[1].Coefficient.Should().Be(-6);
            actual[2].Terms[1].VariableSubscript.Should().Be(2);
            actual[2].Sign.Should().Be(Constraint.ComparisonType.LESSTHAN);
            actual[2].RightHandSide.Should().Be(10);
        }
    }
}
