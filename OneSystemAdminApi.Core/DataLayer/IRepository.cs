using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OneSystemAdminApi.Core.DataLayer
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> Query();

        Task<T> FindAsync(Expression<Func<T, bool>> match);

        Task<T> GetAsync(int id);
        Task<T> GetAsync(long id);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T changes);
        void Update(T changes);

        Task<T> DeleteAsync(int id);
        Task<T> DeleteAsync(long id);
    }
}
