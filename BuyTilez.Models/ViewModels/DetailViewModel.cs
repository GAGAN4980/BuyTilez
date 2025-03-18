namespace BuyTilez.Models.ViewModels
{
    public class DetailViewModel
    {
        public DetailViewModel()
        {
            Product = new Product();
        }

        public Product Product { get; set; }

        public bool ExistsInCart { get; set; }
    }
}
