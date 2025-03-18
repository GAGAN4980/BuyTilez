using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order> 
    {
        void Update(Order order);

        
    }
}
