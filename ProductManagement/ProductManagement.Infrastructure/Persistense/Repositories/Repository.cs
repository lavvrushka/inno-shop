using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Infrastructure.Persistense.Context;

namespace ProductManagement.Infrastructure.Persistense.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ProductManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ProductManagementDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

    }
}
