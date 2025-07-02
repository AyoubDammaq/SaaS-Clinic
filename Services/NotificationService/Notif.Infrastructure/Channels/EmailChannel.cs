using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notif.Domain.Enums;
using System.Net.Mail;
using System.Net;
using Notif.Domain.Channels;
using Notif.Domain.ValueObject;
using Notif.Application.Interfaces;

namespace Notif.Infrastructure.Channels
{
    public class EmailChannel : INotificationChannel
    {
        private readonly IUserEmailResolverService _emailResolver;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailChannel> _logger;

        public NotificationChannel ChannelType => NotificationChannel.Email;

        public EmailChannel(IUserEmailResolverService userEmailResolverService, IConfiguration configuration, ILogger<EmailChannel> logger)
        {
            _emailResolver = userEmailResolverService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAsync(NotificationMessage message)
        {
            var email = await _emailResolver.GetUserEmailAsync(message.RecipientId, message.RecipientType);

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Email not found for user {UserId}", message.RecipientId);
                return false;
            }
            try
            {
                using var client = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_configuration["EmailSettings:Port"] ?? "587"),
                    Credentials = new NetworkCredential(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        _configuration["EmailSettings:SenderEmail"] ?? throw new ArgumentException("SenderEmail is missing"),
                        _configuration["EmailSettings:SenderName"] ?? "Clinic Management"
                    ),
                    Subject = message.Title,
                    Body = message.Content,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {RecipientId}", message.RecipientId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {RecipientId}", message.RecipientId);
                return false;
            }
        }
    }
}
