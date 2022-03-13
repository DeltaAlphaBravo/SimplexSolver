using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MarketTestScenarioGenerator.Tests
{
    [TestClass]
    public class CreateSymbolMapTests
    {
        [TestMethod]
        public void Test1()
        {
            var actual = Program.CreateSymbolMap(8, 9);

            actual.Cast<int>().SequenceEqual(Enumerable.Range(0, 72));
        }

        [TestMethod]
        public void Test2()
        {
            var actual = Program.CreateSymbolMap(8, 9, 1);

            actual.Cast<int>().SequenceEqual(Enumerable.Range(1, 72));
        }
    }
}
