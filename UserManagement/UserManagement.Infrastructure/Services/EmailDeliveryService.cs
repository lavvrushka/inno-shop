using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Infrastructure.Services
{
    public class EmailDeliveryService : IEmailDeliveryService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailDeliveryService(IConfiguration configuration)
        {
            _smtpHost = configuration["Email:SmtpHost"];
            var portString = configuration["Email:SmtpPort"];
            if (string.IsNullOrEmpty(portString))
            {
                throw new ArgumentNullException("SmtpPort", "SmtpPort is not configured in Email settings.");
            }
            _smtpPort = int.Parse(portString);
            _smtpUser = configuration["Email:SmtpUser"];
            _smtpPass = configuration["Email:SmtpPassword"];
        }


        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
