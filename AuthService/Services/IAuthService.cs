﻿using AuthentificationService.Entities;
using AuthentificationService.Models;

namespace AuthentificationService.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<bool> LogoutAsync(Guid userId);
        Task<bool> ChangePasswordAsync(ChangePasswordDto request);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserDetailsByIdAsync(Guid UserId);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequestDto request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(User user, string decodedToken, string newPassword);
    }
}
