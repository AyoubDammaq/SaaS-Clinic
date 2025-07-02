using Notif.Domain.Enums;

namespace Notif.Application.Interfaces
{
    public interface IUserEmailResolverService
    {
        Task<string?> GetUserEmailAsync(Guid userId, UserType userType);
    }
}
