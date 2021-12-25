using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LinearProgramming.Parsing
{
    public class StandardFormLinearProgramParser
    {
        private readonly ConstraintParser _constraintParser;
        private readonly ObjectiveParser _objectiveParser;

        internal StandardFormLinearProgramParser(ConstraintParser constraintParser, ObjectiveParser objectiveParser)
        {
            _constraintParser = constraintParser;
            _objectiveParser = objectiveParser;
        }

        public static StandardFormLinearProgramParser CreateDefault()
        {
            var termParser = new TermParser();
            return new StandardFormLinearProgramParser(new ConstraintParser(termParser), new ObjectiveParser(termParser));
        }

        public LinearProgram Parse(string linearProgram)
        {
            string format = @"^((?:Maximize|Minimize):.+)\s*Subject to: (.+)$";
            Regex r = new(format, RegexOptions.IgnoreCase|RegexOptions.Singleline);
            var match = r.Match(linearProgram);
            var objective = _objectiveParser.ParseObjective(match.Groups[1].Captures[0].Value);
            var constraints = ParseAllConstraints(match.Groups[2].Captures[0].Value);
            return new LinearProgram(objective, constraints);
        }

        private Constraint[] ParseAllConstraints(string constraints)
        {
            List<Constraint> result = new();

            foreach (var constraint in constraints.Split('\n'))
            {
                result.Add(_constraintParser.ParseConstraint(constraint));
            }
            return result.ToArray();
        }
    }
}
