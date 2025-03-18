using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyTilez.Data.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Update(product);
        }

        public IEnumerable<SelectListItem> GetAllDropdownList(string option)
        {
            if (option == Constants.CategoryName)
            {
                return _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.CategoryName,
                    Value = i.Id.ToString()
                });
            }
            if (option == Constants.ApplicationTypeName)
            {
                return _db.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            }

            return null;
        }
    }
}
