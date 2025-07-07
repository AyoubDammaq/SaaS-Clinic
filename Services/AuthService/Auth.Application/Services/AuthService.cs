using Auth.Application.DTOs;
using AuthentificationService.Data;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthentificationService.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
    {
        public async Task<User?> RegisterAsync(UserDto request)
        {
            logger.LogInformation("Tentative d'inscription pour l'email : {Email}", request.Email);
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                logger.LogWarning("Inscription échouée : l'utilisateur avec l'email {Email} existe déjà.", request.Email);
                return null;
            }
            if (!IsStrongPassword(request.Password))
            {
                logger.LogWarning("Inscription échouée : mot de passe trop faible pour l'email {Email}.", request.Email);
                return null;
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PasswordHashed = hashedPassword;
            user.Role = request.Role;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            logger.LogInformation("Utilisateur inscrit avec succès : {Email}", request.Email);

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            logger.LogInformation("Demande de rafraîchissement de token pour l'utilisateur {UserId}", request.UserId);
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                logger.LogWarning("Rafraîchissement de token échoué : token invalide ou expiré pour l'utilisateur {UserId}", request.UserId);
                return null;
            }

            var response = new TokenResponseDto
            {
                AccessToken = GenerateJwtToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            logger.LogInformation("Token rafraîchi avec succès pour l'utilisateur {UserId}", request.UserId);
            return response;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDTO request)
        {
            logger.LogInformation("Tentative de connexion pour l'email : {Email}", request.Email);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                logger.LogWarning("Connexion échouée : utilisateur non trouvé pour l'email {Email}", request.Email);
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHashed, request.Password) == PasswordVerificationResult.Failed)
            {
                logger.LogWarning("Connexion échouée : mot de passe incorrect pour l'email {Email}", request.Email);
                return null;
            }

            var response = new TokenResponseDto
            {
                AccessToken = GenerateJwtToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            logger.LogInformation("Connexion réussie pour l'email : {Email}", request.Email);
            return response;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                logger.LogWarning("Validation du refresh token échouée : utilisateur {UserId} non trouvé.", userId);
                return null;
            }
            if (user.RefreshToken != refreshToken)
            {
                logger.LogWarning("Validation du refresh token échouée : token incorrect pour l'utilisateur {UserId}.", userId);
                return null;
            }
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                logger.LogWarning("Validation du refresh token échouée : token expiré pour l'utilisateur {UserId}.", userId);
                return null;
            }
            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            logger.LogDebug("Nouveau refresh token généré.");
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await context.SaveChangesAsync();
            logger.LogInformation("Refresh token sauvegardé pour l'utilisateur {UserId}", user.Id);
            return refreshToken;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            logger.LogDebug("JWT généré pour l'utilisateur {UserId}", user.Id);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> LogoutAsync(string email)
        {
            logger.LogInformation("Déconnexion demandée pour l'utilisateur {UserId}", email);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                logger.LogWarning("Déconnexion échouée : utilisateur {UserId} non trouvé.", email);
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await context.SaveChangesAsync();
            logger.LogInformation("Déconnexion réussie pour l'utilisateur {UserId}", email);

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto request)
        {
            logger.LogInformation("Changement de mot de passe demandé pour l'utilisateur {UserId}", request.UserId);
            var user = await context.Users.FindAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("Changement de mot de passe échoué : utilisateur {UserId} non trouvé.", request.UserId);
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, request.CurrentPassword) == PasswordVerificationResult.Failed)
            {
                logger.LogWarning("Changement de mot de passe échoué : mot de passe actuel incorrect pour l'utilisateur {UserId}", request.UserId);
                return false;
            }

            if (!IsStrongPassword(request.NewPassword))
            {
                logger.LogWarning("Changement de mot de passe échoué : nouveau mot de passe trop faible pour l'utilisateur {UserId}", request.UserId);
                return false;
            }

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, request.NewPassword) == PasswordVerificationResult.Success)
            {
                logger.LogWarning("Changement de mot de passe échoué : le nouveau mot de passe est identique à l'ancien pour l'utilisateur {UserId}", request.UserId);
                return false;
            }

            user.PasswordHashed = passwordHasher.HashPassword(user, request.NewPassword);
            await context.SaveChangesAsync();
            logger.LogInformation("Mot de passe changé avec succès pour l'utilisateur {UserId}", request.UserId);

            return true;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            logger.LogInformation("Récupération de tous les utilisateurs.");
            var users = await context.Users.ToListAsync();
            logger.LogInformation("{Count} utilisateurs récupérés.", users.Count);
            return users;
        }

        public async Task<User> GetUserDetailsByIdAsync(Guid UserId)
        {
            logger.LogInformation("Récupération des détails de l'utilisateur {UserId}", UserId);
            var user = await context.Users.FirstOrDefaultAsync(m => m.Id == UserId);
            if (user is null)
                logger.LogWarning("Aucun utilisateur trouvé avec l'ID {UserId}", UserId);
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            logger.LogInformation("Suppression de l'utilisateur {UserId} demandée.", userId);
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                logger.LogWarning("Suppression échouée : utilisateur {UserId} non trouvé.", userId);
                return false;
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            logger.LogInformation("Utilisateur {UserId} supprimé avec succès.", userId);

            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeUserRoleRequestDto request)
        {
            logger.LogInformation("Changement de rôle demandé pour l'utilisateur {UserId}", request.UserId);
            var user = await context.Users.FindAsync(request.UserId);
            if (user is null)
            {
                logger.LogWarning("Changement de rôle échoué : utilisateur {UserId} non trouvé.", request.UserId);
                return false;
            }

            user.Role = request.NewRole;
            await context.SaveChangesAsync();
            logger.LogInformation("Rôle de l'utilisateur {UserId} changé en {NewRole}", request.UserId, request.NewRole);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            logger.LogInformation("Demande de réinitialisation de mot de passe pour l'email : {Email}", request.Email);

            // Vérification si l'email est null ou vide
            if (string.IsNullOrEmpty(request.Email))
            {
                logger.LogWarning("Réinitialisation échouée : email non fourni.");
                return false;
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user is null)
            {
                logger.LogWarning("Réinitialisation échouée : utilisateur non trouvé pour l'email {Email}", request.Email);
                return false;
            }

            var resetToken = GenerateRefreshToken();
            user.ResetToken = resetToken;
            user.ResetTokenExpiryTime = DateTime.Now.AddHours(1);
            await context.SaveChangesAsync();

            try
            {
                // Vérification si l'email de l'utilisateur est null avant l'envoi
                if (string.IsNullOrEmpty(user.Email))
                {
                    logger.LogWarning("Réinitialisation échouée : email utilisateur non valide.");
                    return false;
                }

                await SendResetTokenEmailAsync(user.Email, resetToken);
                logger.LogInformation("Email de réinitialisation envoyé à {Email}", user.Email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'envoi de l'email de réinitialisation à {Email}", user.Email);
                return false;
            }

            return true;
        }

        protected virtual async Task SendResetTokenEmailAsync(string email, string resetToken)
        {
            logger.LogDebug("Envoi de l'email de réinitialisation à {Email}", email);
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
                Body = $"Bonjour,\n\nVoici votre jeton de réinitialisation de mot de passe :\n\n{resetToken}\n\nCe lien est valable pendant 1 heure.",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
            logger.LogDebug("Email de réinitialisation envoyé à {Email}", email);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            logger.LogInformation("Recherche de l'utilisateur par email : {Email}", email);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                logger.LogWarning("Aucun utilisateur trouvé pour l'email {Email}", email);
            return user;
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            logger.LogInformation("Demande de réinitialisation de mot de passe pour l'email : {Email}", email);
            var user = await context.Users.FirstOrDefaultAsync(u =>
                u.Email == email &&
                u.ResetToken == resetToken &&
                u.ResetTokenExpiryTime > DateTime.Now);

            if (user is null)
            {
                logger.LogWarning("Réinitialisation échouée : utilisateur non trouvé ou token invalide/expiré pour l'email {Email}", email);
                return false;
            }

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHashed = passwordHasher.HashPassword(user, newPassword);

            user.ResetToken = null;
            user.ResetTokenExpiryTime = null;

            await context.SaveChangesAsync();
            logger.LogInformation("Mot de passe réinitialisé avec succès pour l'email {Email}", email);
            return true;
        }

        private bool IsStrongPassword(string password)
        {
            var options = new PasswordOptions
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = false
            };

            if (password.Length < options.RequiredLength)
            {
                logger.LogDebug("Mot de passe trop court.");
                return false;
            }
            if (options.RequireDigit && !password.Any(char.IsDigit))
            {
                logger.LogDebug("Mot de passe sans chiffre.");
                return false;
            }
            if (options.RequireLowercase && !password.Any(char.IsLower))
            {
                logger.LogDebug("Mot de passe sans minuscule.");
                return false;
            }
            if (options.RequireUppercase && !password.Any(char.IsUpper))
            {
                logger.LogDebug("Mot de passe sans majuscule.");
                return false;
            }
            if (options.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
            {
                logger.LogDebug("Mot de passe sans caractère spécial.");
                return false;
            }

            return true;
        }
    }
}
