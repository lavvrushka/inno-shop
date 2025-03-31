namespace ProductManagement.Domain.Interfaces.IServices
{
    public interface IConnectionService
    {
        Task<bool> UserExistsAndIsActiveAsync(Guid userId);
    }
}
