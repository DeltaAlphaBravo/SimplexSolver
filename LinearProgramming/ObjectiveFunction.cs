using System.Collections.Generic;

namespace LinearProgramming
{
    public class ObjectiveFunction
    {
        public Optimization Optimization { get; init; }
        public IList<Term> Terms { get; init; } = new List<Term>();
    }
}