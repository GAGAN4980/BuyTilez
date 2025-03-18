using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BuyTilez.Data.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        IEnumerable<T> GetAll(
            // Search filter
            Expression<Func<T, bool>> filter = null,
            // Sorting function
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            // Properties to include
            string includeProperties = null,
            // Object tracking
            bool isTracking = true
        );

        T GetFirstOrDefault(
            // Search filter
            Expression<Func<T, bool>> filter = null,
            // Properties to include
            string includeProperties = null,
            // Object tracking
            bool isTracking = true
        );

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        void Save();
    }
}
