using BuyTilez.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

        IEnumerable<SelectListItem> GetAllDropdownList(string option);
    }
}
