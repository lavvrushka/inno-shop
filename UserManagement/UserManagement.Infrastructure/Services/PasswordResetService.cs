using System.Security.Cryptography;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Infrastructure.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailDeliveryService _emailDeliveryService;

        public PasswordResetService(IUnitOfWork unitOfWork, IEmailDeliveryService emailDeliveryService)
        {
            _unitOfWork = unitOfWork;
            _emailDeliveryService = emailDeliveryService;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
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
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1); 

                await _unitOfWork.Users.UpdateUserAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            return token;
        }

        public async Task SendPasswordResetEmailAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var token = await GeneratePasswordResetTokenAsync(userId);
            var subject = "Reset Your Password";
            var body = $"<p>Your Reset Password recovery token is: <strong>{token}</strong></p>";

            await _emailDeliveryService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.PasswordResetToken != token || user.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                return false;
            }
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            await _unitOfWork.Users.UpdateUserAsync(user);
            return true;
        }
    }
}
