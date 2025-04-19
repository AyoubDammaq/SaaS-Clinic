using AuthentificationService.Entities;

namespace AuthentificationService.Models
{
    public class ChangeUserRoleRequestDto
    {
        public Guid UserId { get; set; }
        public UserRole NewRole { get; set; }
    }
}
