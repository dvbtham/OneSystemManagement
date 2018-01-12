using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OneSystemAdminApi.Core.DataLayer
{
    public class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        private readonly OneSystemDbContext _dbContext;
        private bool _disposed;

        public Repository(OneSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Query()
        {
            return _dbContext.Set<T>();
        }

        public Task<T> GetAsync(int id)
        {
            return _dbContext.Set<T>().FindAsync(id);
        }
        public Task<T> GetAsync(long id)
        {
            return _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(match);
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T changes)
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            await _dbContext.SaveChangesAsync();
        }

        public void Update(T changes)
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

             _dbContext.SaveChanges();
        }

        public async Task<T> DeleteAsync(int id)
        {
            var entity = await GetAsync(id);

            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);

                await _dbContext.SaveChangesAsync();
            }

            return entity;
        }

        public async Task<T> DeleteAsync(long id)
        {
            var entity = await GetAsync(id);

            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);

                await _dbContext.SaveChangesAsync();
            }

            return entity;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();

                    _disposed = true;
                }
            }
        }
    }
}
