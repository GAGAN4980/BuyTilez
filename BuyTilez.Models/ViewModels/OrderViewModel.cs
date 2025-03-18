namespace BuyTilez.Models.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public IEnumerable<OrderDetails> Details { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
