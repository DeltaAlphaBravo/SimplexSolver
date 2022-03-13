namespace MarketTestScenarioGenerator
{
    public class Buyer
    {
        private readonly decimal _bidPrice;
        private double _demand;
        private readonly string _name;

        public string Name => _name;

        public Buyer(string name, double demand, decimal bidPrice)
        {
            _bidPrice = bidPrice;
            _name = name;
            _demand = demand;
        }

        public decimal GetBidPrice() => _bidPrice;
        public double GetDemand() => _demand;
    }
}
