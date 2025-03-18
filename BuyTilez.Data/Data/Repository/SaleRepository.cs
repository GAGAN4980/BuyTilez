using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        private readonly ApplicationDbContext _db;

        public SaleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Sale sale)
        {
            _db.Update(sale);
        }
    }
}
