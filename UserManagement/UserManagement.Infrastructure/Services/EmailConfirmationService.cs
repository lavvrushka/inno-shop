using System.Security.Cryptography;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Infrastructure.Services
{
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailDeliveryService _emailService;
        

        public EmailConfirmationService(
            IUnitOfWork unitOfWork,
            IEmailDeliveryService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var user = await _unitOfWork.Users.GetByEmailConfirmationTokenAsync(token);
            if (user == null || user.EmailVerifiedAt != null)
                return false;  

            user.EmailVerifiedAt = DateTime.UtcNow;
            user.EmailConfirmationToken = null;
            await _unitOfWork.Users.UpdateUserAsync(user);

            return true;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
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


            await _unitOfWork.Users.UpdateEmailConfirmationTokenAsync(userId, token);

            return token;
        }

        public async Task SendConfirmationEmailAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var token = await GenerateEmailConfirmationTokenAsync(userId);
            var confirmationLink = $"http://localhost:3000/confirm-email/{token}";

            var subject = "Confirm Your Email";
            var body = $"<p>Please confirm your email by clicking on the link below:</p><a href='{confirmationLink}'>Confirm Email</a>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task<bool> ValidateConfirmationTokenAsync(string token)
        {
            var user = await _unitOfWork.Users.GetByEmailConfirmationTokenAsync(token);
            if (user == null || user.EmailVerifiedAt != null)
            {
                return false; 
            }

            await ConfirmEmailAsync(token);
            return true;
        }
    }
}
