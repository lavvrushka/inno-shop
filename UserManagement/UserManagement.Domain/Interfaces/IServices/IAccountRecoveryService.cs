namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IAccountRecoveryService
    {
        Task<string> GenerateAccountRecoveryTokenAsync(Guid userId);
        Task SendAccountRecoveryEmailAsync(Guid userId);
        Task RecoverAccountAsync(Guid userId, string token);
    }
}
