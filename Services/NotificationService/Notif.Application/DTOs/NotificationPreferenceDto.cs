using Notif.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notif.Application.DTOs
{
    public record NotificationPreferenceDto(
        Guid UserId,
        UserType UserType,
        List<NotificationChannel> PreferredChannels,
        Dictionary<NotificationType, bool> NotificationSettings,
        string Language,
        string? PhoneNumber,
        string? Email
    );

}
