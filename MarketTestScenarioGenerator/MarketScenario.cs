using LinearProgramming;
using System.Collections.Generic;
using System.Linq;

namespace MarketTestScenarioGenerator
{
    public class MarketScenario
    {
        private IEnumerable<PotentialTransaction> _potentialTransactions;
        public MarketScenario(IEnumerable<Buyer> buyers, IEnumerable<Seller> sellers)
        {
            var potentialTransactions = new List<PotentialTransaction>();
            var variableIndex = 0;
            foreach (var buyer in buyers)
            {
                foreach (var seller in sellers)
                {
                    var potentialTransaction = new PotentialTransaction
                    {
                        Buyer = buyer,
                        Seller = seller,
                    };
                    if (potentialTransaction.PriceDifference > 0)
                    {
                        potentialTransaction.SaleVariableIndex = ++variableIndex;
                        potentialTransactions.Add(potentialTransaction);
                    }
                }
            }
            foreach (var potentialTransaction in potentialTransactions)
                potentialTransaction.SlackVariableIndex = ++variableIndex;
            _potentialTransactions = potentialTransactions;
        }

        public IEnumerable<Transaction> ReadSolution(LinearProgramResult result)
        {
            return _potentialTransactions.Where(t => result.SolutionValues.Any(s => s.VariableSubscript == t.SaleVariableIndex && s.Coefficient > 0))
                                         .Select(x => new Transaction { Buyer = x.Buyer, Seller = x.Seller, QuantitySold = result.SolutionValues.Single(s => s.VariableSubscript == x.SaleVariableIndex).Coefficient });
        }

        public LinearProgram ToLinearProgram()
        {
            var constraints = CreatePriceConstraints()
                              .Concat(CreateProductionConstraints())
                              .Concat(CreateDemandConstraints())
                              .Concat(CreatePositiveQuantityConstraints());
            var objective = new ObjectiveFunction
            {
                Optimization = Optimization.Maximize,
                Terms = _potentialTransactions.Select(x => new Term { Coefficient = 1, VariableSubscript = x.SlackVariableIndex }).ToList()
            };
            return new LinearProgram(objective, constraints.ToArray());
        }

        private IEnumerable<Constraint> CreatePriceConstraints()
        {
            foreach (var potentialTransaction in _potentialTransactions)
            {
                var constraint = new Constraint(1,
                            Constraint.ComparisonType.EQUALTO,
                            new[] {
                                new Term { Coefficient = potentialTransaction.PriceDifference, VariableSubscript = potentialTransaction.SaleVariableIndex },
                                new Term { Coefficient = -1, VariableSubscript = potentialTransaction.SlackVariableIndex }
                            });
                yield return constraint;
            }
        }

        private IEnumerable<Constraint> CreateProductionConstraints()
        {
            foreach (var sellerTransactions in _potentialTransactions.GroupBy(t => t.Seller))
            {
                var terms = new List<Term>();
                foreach (var transaction in sellerTransactions)
                    terms.Add(new Term { Coefficient = 1, VariableSubscript = transaction.SaleVariableIndex });
                yield return new Constraint(sellerTransactions.Key.GetQuantity(), Constraint.ComparisonType.LESSTHAN, terms);
            }
        }

        private IEnumerable<Constraint> CreateDemandConstraints()
        {
            foreach (var buyerTransactions in _potentialTransactions.GroupBy(t => t.Buyer))
            {
                var terms = new List<Term>();
                foreach (var transaction in buyerTransactions)
                    terms.Add(new Term { Coefficient = 1, VariableSubscript = transaction.SaleVariableIndex });
                yield return new Constraint(buyerTransactions.Key.GetDemand(), Constraint.ComparisonType.LESSTHAN, terms);
            }
        }

        private IEnumerable<Constraint> CreatePositiveQuantityConstraints()
        {
            return _potentialTransactions.Select(x =>
            new Constraint(0,
            Constraint.ComparisonType.GREATERTHAN,
            new[]
            {
                new Term { Coefficient = 1, VariableSubscript = x.SaleVariableIndex }
            }));
        }
    }
}
