namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IConnectionService
    {
        Task<bool> HideUserProductsAsync(Guid userId);

        Task<bool> ShowUserProductsAsync(Guid userId);

        Task<bool> DeleteAllProductsByUserAsync(Guid userId);
    }
}
