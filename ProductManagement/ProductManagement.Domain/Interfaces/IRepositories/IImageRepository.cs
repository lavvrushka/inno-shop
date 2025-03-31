using ProductManagement.Domain.Models;

namespace ProductManagement.Domain.Interfaces.IRepositories
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<Guid> AddImageToProductAsync(Image entity);
    }
}
