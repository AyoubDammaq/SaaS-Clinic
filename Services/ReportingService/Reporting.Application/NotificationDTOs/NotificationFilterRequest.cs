using Notif.Domain.Enums;

namespace Notif.Application.DTOs
{
    public record NotificationFilterRequest(
        Guid? RecipientId = null,
        UserType? RecipientType = null,
        NotificationStatus? Status = null,
        NotificationType? Type = null,
        DateTime? From = null,
        DateTime? To = null,
        int Page = 1,
        int PageSize = 20
    );
}
