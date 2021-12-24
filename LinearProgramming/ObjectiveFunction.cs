using System.Collections.Generic;

namespace LinearProgramming
{
    public class ObjectiveFunction
    {
        public Optimization Optimization { get; }
        public List<Term> Terms { get; set; } = new List<Term>();
    }
}