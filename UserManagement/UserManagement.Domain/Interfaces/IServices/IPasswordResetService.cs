namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IPasswordResetService
    {
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token);
        Task SendPasswordResetEmailAsync(Guid userId);
    }
}
