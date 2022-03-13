namespace LinearProgramming.Parsing
{
    internal class TermParser
    {
        public Term ParseTerm(string term)
        {
            int xIndex = term.IndexOf('x');
            return new Term
            {
                Coefficient = double.Parse(term[..xIndex]),
                VariableSubscript = int.Parse(term[(xIndex + 1)..])
            };
        }
    }
}
