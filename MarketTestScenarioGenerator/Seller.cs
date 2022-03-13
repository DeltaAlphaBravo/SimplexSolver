namespace MarketTestScenarioGenerator
{
    public class Seller
    {
        private readonly decimal _sellPrice;
        private readonly string _name;
        private double _quantity;
        public string Name => _name;

        public Seller(string name, double quantity, decimal sellPrice)
        {
            _sellPrice = sellPrice;
            _quantity = quantity;
            _name = name;
        }

        public decimal GetSellPrice() => _sellPrice;

        public double GetQuantity() => _quantity;
    }
}
