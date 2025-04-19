using AuthentificationService.Data;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthentificationService.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.Email = request.Email;
            user.PasswordHashed = hashedPassword;
            user.Role = UserRole.Patient;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            var response = new TokenResponseDto
            {
                AccessToken = GenerateJwtToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            return response;
        }


        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHashed, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var response = new TokenResponseDto
            {
                AccessToken = GenerateJwtToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };


            return response;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            return user;
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);
            await context.SaveChangesAsync();
            return refreshToken;
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDto request)
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user is null)
            {
                return false;
            }

            if (user.RefreshToken is null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();
            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, request.CurrentPassword) == PasswordVerificationResult.Failed)
            {
                return false;
            }

            var newPasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            if (passwordHasher.VerifyHashedPassword(user, newPasswordHash, request.CurrentPassword) == PasswordVerificationResult.Success)
            {
                return false;
            }

            user.PasswordHashed = newPasswordHash;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> GetUserDetailsByIdAsync(Guid UserId)
        {
            return await context.Users.FirstOrDefaultAsync(m => m.Id == UserId);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return false;
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequestDto request)
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user is null)
            {
                return false;
            }

            user.Role = request.NewRole;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                return false;
            }

            var resetToken = GenerateRefreshToken();
            // Save the reset token and its expiry time to the user entity
            GenerateAndSaveRefreshTokenAsync(user);

            // Send the reset token to the user's email
            await SendResetTokenEmailAsync(user.Email, resetToken);

            return true;
        }

        private async Task SendResetTokenEmailAsync(string email, string resetToken)
        {
            // Logique d'envoi d'email
            var smtpClient = new SmtpClient(configuration["Smtp:Host"])
            {
                Port = 587,
                Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(configuration["Smtp:Sender"]),
                Subject = "Réinitialisation de mot de passe",
                Body = $"Votre jeton de réinitialisation de mot de passe est : {resetToken}",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task<User?> FindByEmailAsync(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ResetPasswordAsync(User user, string decodedToken, string newPassword)
        {
            var passwordHasher = new PasswordHasher<User>();
            var newPasswordHash = passwordHasher.HashPassword(user, newPassword);
            user.PasswordHashed = newPasswordHash;
            await context.SaveChangesAsync();
            return true;
        }



    }
}
