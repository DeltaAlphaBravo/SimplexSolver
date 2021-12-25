using System;

namespace LinearProgramming
{
    public class LinearProgram
    {
        public LinearProgram(ObjectiveFunction objective, Constraint[] constraints)
        {
            Objective = objective ?? throw new ArgumentNullException(nameof(objective));
            Constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
        }

        public ObjectiveFunction Objective { get; }
        public Constraint[] Constraints { get; }
    }
}
