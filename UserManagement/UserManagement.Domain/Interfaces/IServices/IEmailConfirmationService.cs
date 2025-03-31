namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IEmailConfirmationService
    {
        Task<bool> ConfirmEmailAsync(string token);
        Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
        Task SendConfirmationEmailAsync(Guid userId);
        Task<bool> ValidateConfirmationTokenAsync(string token);
    }
}
