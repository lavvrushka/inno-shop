namespace ProductManagement.Domain.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IImageRepository Images { get; }
        Task<int> SaveChangesAsync();
    }
}
