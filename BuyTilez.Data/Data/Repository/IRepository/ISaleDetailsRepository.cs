using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface ISaleDetailsRepository : IRepository<SaleDetails>
    {
        void Update(SaleDetails saleDetails);
    }
}
