using AuthentificationService.Entities;

namespace Auth.Application.DTOs
{
    public class LinkUserProfileDto
    {
        public Guid UserId { get; set; }
        public Guid EntityId { get; set; }
        public UserRole Role { get; set; }
    }
}
