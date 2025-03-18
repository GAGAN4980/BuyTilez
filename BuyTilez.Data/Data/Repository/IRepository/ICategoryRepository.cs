using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
