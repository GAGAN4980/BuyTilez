using BuyTilez.Data.Data;
using BuyTilez.Data.Data.Repository;
using BuyTilez.Models;

public class CartRepository : Repository<ShoppingCart>, ICartRepository
{
    private readonly ApplicationDbContext _db;

    public CartRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(ShoppingCart shoppingCart)
    {
        _db.Update(shoppingCart);
    }

    public IEnumerable<ShoppingCart> GetUserCart(string userId)
    {
        return _db.Carts.Where(c => c.UserId == userId).ToList();
    }

    public void Add(ShoppingCart cart)
    {
        _db.Carts.Add(cart);
    }

    public void AddRange(List<ShoppingCart> cartList)
    {
        _db.Carts.AddRange(cartList);
    }

    public void ClearUserCart(string userId)
    {
        var userCart = _db.Carts.Where(c => c.UserId == userId);
        _db.Carts.RemoveRange(userCart);
    }
}
