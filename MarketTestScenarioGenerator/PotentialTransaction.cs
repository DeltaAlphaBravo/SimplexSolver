namespace MarketTestScenarioGenerator
{
    internal class PotentialTransaction
    {
        public Buyer Buyer { get; init; }
        public Seller Seller { get; init; }
        public double PriceDifference => (double)(Buyer.GetBidPrice() - Seller.GetSellPrice());
        public int SaleVariableIndex { get; set; }
        public int SlackVariableIndex { get; set; }
    }
}
