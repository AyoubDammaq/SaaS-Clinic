using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notif.Domain.Channels;
using Notif.Domain.Enums;
using Notif.Domain.ValueObject;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Notif.Infrastructure.Channels
{
    public class SmsChannel : INotificationChannel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsChannel> _logger;

        public NotificationChannel ChannelType => NotificationChannel.SMS;

        public SmsChannel(IConfiguration configuration, ILogger<SmsChannel> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<bool> SendAsync(NotificationMessage message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.PhoneNumber))
                {
                    _logger.LogWarning("No phone number available for user {UserId}", message.RecipientId);
                    return false;
                }

                var from = new PhoneNumber(_configuration["Twilio:FromPhoneNumber"]);
                var to = new PhoneNumber(message.PhoneNumber);

                var messageResource = await MessageResource.CreateAsync(
                    body: $"{message.Title}\n\n{message.Content}",
                    from: from,
                    to: to);

                _logger.LogInformation("SMS sent successfully. SID: {MessageSid}", messageResource.Sid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {RecipientId}", message.RecipientId);
                return false;
            }
        }
    }
}
