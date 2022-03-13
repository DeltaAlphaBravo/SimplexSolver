namespace MarketTestScenarioGenerator
{
    public class Transaction
    {
        public Buyer Buyer { get; internal init; }
        public Seller Seller { get; internal init; }
        public double QuantitySold { get; internal init; }
        public decimal SalePrice { get; internal init; }
    }
}
