using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming
{
    public class SimplexSolver
    {
        public LinearProgramResult GetSolution(LinearProgram linearProgram)
        {
            var tableau = Tableau.Create(linearProgram.Constraints, linearProgram.Objective, UserVariableCount(linearProgram));

            if (tableau.Solve(linearProgram.Objective.Optimization == Optimization.Minimize))
                return new LinearProgramResult()
                {
                    SolutionValues = tableau.GetSolutionValues().Where((_, i) => i > 0)
                                                                .Select((d, i) => new Term 
                                                                                  {
                                                                                    Coefficient = d, 
                                                                                    VariableSubscript = i + 1
                                                                                  }),
                    OptimalValue = tableau.GetSolutionValues().Where((_, i) => i == 0).Single()
                };
            return new LinearProgramResult();
        }
        private static int UserVariableCount(LinearProgram linearProgram)
        {
            var uniqueVariables = new HashSet<int>();
            var variables = linearProgram.Constraints.SelectMany(c => c.Terms)
                                                     .Concat(linearProgram.Objective.Terms)
                                                     .Select(t => t.VariableSubscript);
            foreach (var variable in variables)
            {
                uniqueVariables.Add(variable);
            }
            return uniqueVariables.Count();
        }
    }
}
