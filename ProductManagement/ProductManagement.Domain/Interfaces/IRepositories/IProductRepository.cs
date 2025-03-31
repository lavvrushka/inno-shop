using ProductManagement.Domain.Models;
using System.Linq.Expressions;

namespace ProductManagement.Domain.Interfaces.IRepositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<List<Product>> GetFilteredAsync(Expression<Func<Product, bool>> filter);
        Task HideProductsByUserIdAsync(Guid userId);
        Task RestoreProductsByOwnerIdAsync(Guid userId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<List<Product>> GetByPageAsync(PageSettings pageSettings);
        Task<int> GetProductCountAsync();
        Task DeleteRange(IEnumerable<Product> products);
    }
}
