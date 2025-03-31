namespace UserManagement.Domain.Interfaces.IServices
{
    public interface IEmailDeliveryService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
