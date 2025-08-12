using Notif.Domain.Enums;

namespace Notif.Application.DTOs
{
    public record NotificationSummaryDto(
        Guid Id,
        string Title,
        NotificationStatus Status,
        DateTime CreatedAt,
        DateTime? SentAt
    );
}
