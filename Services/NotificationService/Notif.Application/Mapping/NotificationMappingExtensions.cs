using Notif.Application.DTOs;
using Notif.Domain.Entities;
using Notif.Domain.ValueObject;

namespace Notif.Application.Mapping
{
    public static class NotificationMappingExtensions
    {
        public static NotificationDto ToDto(this Notification n) => new(
            Id: n.Id,
            RecipientId: n.RecipientId,
            RecipientType: n.RecipientType,
            Type: n.Type,
            Channel: n.Channel,
            Title: n.Title,
            Content: n.Content,
            Priority: n.Priority,
            Status: n.Status,
            CreatedAt: n.CreatedAt,
            SentAt: n.SentAt
        );

        public static NotificationSummaryDto ToSummaryDto(this Notification n) => new(
            Id: n.Id,
            Title: n.Title,
            Status: n.Status,
            CreatedAt: n.CreatedAt,
            SentAt: n.SentAt
        );

        public static NotificationPreferenceDto ToDto(this UserNotificationPreferences p) => new(
            UserId: p.UserId,
            UserType: p.UserType,
            PreferredChannels: p.PreferredChannels,
            NotificationSettings: p.NotificationSettings,
            Language: p.Language,
            PhoneNumber: p.PhoneNumber,
            Email: p.Email
        );
    }
}
