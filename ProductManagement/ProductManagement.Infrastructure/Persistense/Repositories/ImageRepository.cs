using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Models;
using ProductManagement.Infrastructure.Persistense.Context;

namespace ProductManagement.Infrastructure.Persistense.Repositories
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        public ImageRepository(ProductManagementDbContext context) : base(context) { }

        public async Task<Guid> AddImageToProductAsync(Image image)
        {
            await _context.Set<Image>().AddAsync(image);
            return image.Id;
        }
    }
}
