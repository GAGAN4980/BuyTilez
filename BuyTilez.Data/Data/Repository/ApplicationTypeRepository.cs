using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;

namespace BuyTilez.Data.Data.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationType applicationType)
        {
            var existingType = _db.ApplicationTypes.FirstOrDefault(c => c.Id == applicationType.Id);
            if (existingType != null)
            {
                existingType.Name = applicationType.Name;
            }
        }
    }
}
