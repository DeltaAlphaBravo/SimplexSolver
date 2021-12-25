using System.Collections.Generic;

namespace LinearProgramming
{
    public class LinearProgramResult
    {
        public double? OptimalValue { get; init; }
        public IEnumerable<Term> SolutionValues { get; init; }
    }
}
