using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface ISaleRepository : IRepository<Sale>
    {
        void Update(Sale sale);
    }
}
