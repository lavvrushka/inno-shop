using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;
using ProductManagement.Infrastructure.Persistense.Context;
using System.Linq.Expressions;

namespace ProductManagement.Infrastructure.Persistense.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ProductManagementDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .Include(e => e.Image)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
          
            return await _dbSet
                .Where(p => !p.IsDeleted &&
                           (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .Include(e => e.Image)
                .ToListAsync();
        }
        public async Task<List<Product>> GetFilteredAsync(Expression<Func<Product, bool>> filter)
        {
            return await _dbSet
                .Where(p => !p.IsDeleted)
                .Where(filter)
                .Where(p => !p.IsDeleted)
                .Include(p => p.Image)
                .ToListAsync();
        }

        public async Task HideProductsByUserIdAsync(Guid userId)
        {
           
            var products = await _dbSet.Where(p => p.UserId == userId && !p.IsDeleted).Include(e => e.Image).ToListAsync();
            foreach (var product in products)
            {
                product.IsDeleted = true;
            }
            
        }

        public async Task RestoreProductsByOwnerIdAsync(Guid userId)
        {  
            var products = await _dbSet.Where(p => p.UserId == userId && p.IsDeleted).Include(e => e.Image).ToListAsync();
            foreach (var product in products)
            {
                product.IsDeleted = false;
            }
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Image)
                .Where(p => !p.IsDeleted) 
                .ToListAsync();
        }

        public async Task<List<Product>> GetByPageAsync(PageSettings pageSettings)
        {
            return await _context.Products
                .Include(e => e.Image)
                .AsNoTracking()
                .Skip((pageSettings.PageIndex - 1) * pageSettings.PageSize)
                .Take(pageSettings.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }
        public new async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(e => e.Image)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task DeleteRange(IEnumerable<Product> products)
        {
            _dbSet.RemoveRange(products);
        }
    }
}
