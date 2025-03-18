using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository
{
    public class SaleDetailsRepository : Repository<SaleDetails>, ISaleDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public SaleDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SaleDetails saleDetails)
        {
            _db.Update(saleDetails);
        }
    }
}
