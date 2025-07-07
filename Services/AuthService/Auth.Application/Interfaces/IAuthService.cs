using Auth.Application.DTOs;
using AuthentificationService.Entities;
using AuthentificationService.Models;

namespace AuthentificationService.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(LoginRequestDTO request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> LogoutAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordDto request);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserDetailsByIdAsync(Guid UserId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequestDto request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword);
    }
}
