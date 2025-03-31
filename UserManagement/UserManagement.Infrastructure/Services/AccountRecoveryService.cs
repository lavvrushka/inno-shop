using System.Security.Cryptography;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Infrastructure.Services
{
    public class AccountRecoveryService : IAccountRecoveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailDeliveryService _emailDeliveryService;

        public AccountRecoveryService(IUnitOfWork unitOfWork, IEmailDeliveryService emailDeliveryService)
        {
            _unitOfWork = unitOfWork;
            _emailDeliveryService = emailDeliveryService;
        }

        public async Task<string> GenerateAccountRecoveryTokenAsync(Guid userId)
        {
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            var token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user != null)
            {
                user.AccountRecoveryToken = token;
                user.AccountRecoveryTokenExpires = DateTime.UtcNow.AddHours(1); 

                await _unitOfWork.Users.UpdateUserAsync(user);
            }

            return token;
        }

        public async Task SendAccountRecoveryEmailAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var token = user.AccountRecoveryToken;
            var subject = "Recover Your Account";
            var body = $"<p>Your account recovery token is: <strong>{token}</strong></p>";

            await _emailDeliveryService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task RecoverAccountAsync(Guid userId, string token)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (user.AccountRecoveryToken != token || user.AccountRecoveryTokenExpires < DateTime.UtcNow)
                throw new InvalidOperationException("Invalid or expired recovery token.");

            user.IsActive = true;
            user.AccountRecoveryToken = null;
            user.AccountRecoveryTokenExpires = null;

            await _unitOfWork.Users.UpdateUserAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
