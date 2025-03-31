using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Infrastructure.Persistense.Context;
using ProductManagement.Infrastructure.Persistense.Repositories;

namespace ProductManagement.Infrastructure.Persistense
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductManagementDbContext _context;
        private IProductRepository? _productRepository;
        private IImageRepository? _imageRepository;
        public UnitOfWork(ProductManagementDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public IImageRepository Images => _imageRepository ??= new ImageRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
