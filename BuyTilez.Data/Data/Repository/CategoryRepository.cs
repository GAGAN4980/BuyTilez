using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var existingCategory = _db.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
