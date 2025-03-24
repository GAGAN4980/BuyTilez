using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

public interface ICartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart shoppingCart);
    IEnumerable<ShoppingCart> GetUserCart(string userId);
    void Add(ShoppingCart cart);

    void AddRange(List<ShoppingCart> cartList);
    void ClearUserCart(string userId);
}
