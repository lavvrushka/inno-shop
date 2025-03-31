using UserManagement.Domain.Models;

namespace UserManagement.Domain.Interfaces.IRepositories
{

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<int> GetUserCountAsync();
        Task<bool> IsUserActiveAsync(Guid userId);
        Task<bool> CheckEmailExistsAsync(string email);
        Task UpdateUserAsync(User user);
        Task SetUserStatusAsync(Guid userId, bool isActive);
        Task<User?> GetByPasswordResetTokenAsync(string token);
        Task<User?> GetByEmailConfirmationTokenAsync(string token);
        Task ConfirmEmailAsync(Guid userId);
        Task ResetPasswordAsync(Guid userId, string newPasswordHash);
        Task UpdateEmailConfirmationTokenAsync(Guid userId, string token);
        Task<User?> GetByAccountRecoveryTokenAsync(string token);
    }
}
