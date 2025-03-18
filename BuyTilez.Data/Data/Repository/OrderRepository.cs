using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order order)
        {
            _db.Update(order);
        }
    }
}
