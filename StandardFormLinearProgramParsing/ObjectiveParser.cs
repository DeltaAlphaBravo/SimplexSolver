using System;
using System.Collections.Generic;

namespace LinearProgramming.Parsing
{
    internal class ObjectiveParser
    {
        private readonly TermParser _termParser;

        public ObjectiveParser(TermParser termParser)
        {
            _termParser = termParser;
        }
        public ObjectiveFunction ParseObjective(string anObjective)
        {
            anObjective = anObjective.Replace("\\s", ""); //clean the whitespace out.
            var parts = anObjective.Split(':');
            var minMax = Enum.Parse<Optimization>(parts[0]);
            var coeffList = ParseOjectiveFunction(parts[1]);

            return new ObjectiveFunction() { Terms = coeffList, Optimization = minMax };
        }

        private List<Term> ParseOjectiveFunction(string anObjective)
        {
            List<Term> coeffList = new();
            var terms = anObjective.Split('+');

            foreach (var term in terms)
            {
                coeffList.Add(_termParser.ParseTerm(term));
            }

            return coeffList;
        }
    }
}
