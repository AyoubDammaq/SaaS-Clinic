using Auth.Application.DTOs;
using AuthentificationService.Data;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using AuthentificationService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Auth.Tests
{
    public class AuthServiceTests
    {
        private readonly AuthService _service;
        private readonly UserDbContext _context;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock = new Mock<ILogger<AuthService>>();

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new UserDbContext(options);

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"]).Returns("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");
            _configMock.Setup(c => c["Smtp:Host"]).Returns("smtp.gmail.com");
            _configMock.Setup(c => c["Smtp:Username"]).Returns("user");
            _configMock.Setup(c => c["Smtp:Password"]).Returns("pass");
            _configMock.Setup(c => c["Smtp:Sender"]).Returns("test@test.com");

            _service = new AuthService(_context, _configMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenValid()
        {
            var dto = new UserDto
            {
                FullName = "Test User",
                Email = "test@test.com",
                Password = "Password1",
                ConfirmPassword = "Password1"
            };

            var user = await _service.RegisterAsync(dto);

            Assert.NotNull(user);
            Assert.Equal(dto.Email, user.Email);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnNull_WhenUserExists()
        {
            var dto = new UserDto
            {
                FullName = "Test User",
                Email = "test2@test.com",
                Password = "Password1",
                ConfirmPassword = "Password1"
            };
            _context.Users.Add(new User { Email = dto.Email, PasswordHashed = "hash" });
            await _context.SaveChangesAsync();

            var user = await _service.RegisterAsync(dto);

            Assert.Null(user);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnNull_WhenPasswordIsWeak()
        {
            var dto = new UserDto
            {
                FullName = "Weak User",
                Email = "weak@test.com",
                Password = "weak", // trop court, pas de majuscule, pas de chiffre
                ConfirmPassword = "weak",
                Role = UserRole.Patient
            };

            var user = await _service.RegisterAsync(dto);

            Assert.Null(user);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WithCustomRole()
        {
            var dto = new UserDto
            {
                FullName = "Doctor User",
                Email = "doctor@test.com",
                Password = "Password1",
                ConfirmPassword = "Password1",
                Role = UserRole.Doctor
            };

            var user = await _service.RegisterAsync(dto);

            Assert.NotNull(user);
            Assert.Equal(UserRole.Doctor, user.Role);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "login@test.com",
                PasswordHashed = passwordHasher.HashPassword(null, "Password1"),
                FullName = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new LoginRequestDTO { Email = user.Email, Password = "Password1" };
            var result = await _service.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenCredentialsInvalid()
        {
            var dto = new LoginRequestDTO { Email = "notfound@test.com", Password = "wrong" };
            var result = await _service.LoginAsync(dto);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldReturnNull_WhenInvalidToken()
        {
            var dto = new RefreshTokenRequestDto { UserId = Guid.NewGuid(), RefreshToken = "invalid" };
            var result = await _service.RefreshTokensAsync(dto);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldReturnToken_WhenValid()
        {
            var user = new User
            {
                Email = "refresh@test.com",
                PasswordHashed = "hash",
                FullName = "User",
                RefreshToken = "token",
                RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new RefreshTokenRequestDto { UserId = user.Id, RefreshToken = "token" };
            var result = await _service.RefreshTokensAsync(dto);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnTrue_WhenUserExists()
        {
            var user = new User { Email = "logout@test.com", PasswordHashed = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.LogoutAsync(user.Email);
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnTrue_WhenValid()
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "changepass@test.com",
                PasswordHashed = passwordHasher.HashPassword(null, "OldPassword1"),
                FullName = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new ChangePasswordDto
            {
                UserId = user.Id,
                CurrentPassword = "OldPassword1",
                NewPassword = "NewPassword1"
            };

            var result = await _service.ChangePasswordAsync(dto);
            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var dto = new ChangePasswordDto
            {
                UserId = Guid.NewGuid(),
                CurrentPassword = "any",
                NewPassword = "any"
            };
            var result = await _service.ChangePasswordAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFalse_WhenNewPasswordIsWeak()
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "weakpass@test.com",
                PasswordHashed = passwordHasher.HashPassword(null, "OldPassword1"),
                FullName = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new ChangePasswordDto
            {
                UserId = user.Id,
                CurrentPassword = "OldPassword1",
                NewPassword = "weak"
            };

            var result = await _service.ChangePasswordAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnFalse_WhenNewPasswordIsSameAsOld()
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "samepass@test.com",
                PasswordHashed = passwordHasher.HashPassword(null, "Password1"),
                FullName = "User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new ChangePasswordDto
            {
                UserId = user.Id,
                CurrentPassword = "Password1",
                NewPassword = "Password1"
            };

            var result = await _service.ChangePasswordAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUsers()
        {
            _context.Users.Add(new User { Email = "user1@test.com", PasswordHashed = "hash" });
            await _context.SaveChangesAsync();

            var users = await _service.GetAllUsersAsync();
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task GetUserDetailsByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { Email = "details@test.com", PasswordHashed = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserDetailsByIdAsync(user.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserDetailsByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = await _service.GetUserDetailsByIdAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserExists()
        {
            var user = new User { Email = "delete@test.com", PasswordHashed = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteUserAsync(user.Id);
            Assert.True(result);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldReturnTrue_WhenUserExists()
        {
            var user = new User { Email = "role@test.com", PasswordHashed = "hash", Role = UserRole.Patient };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new ChangeUserRoleRequestDto { UserId = user.Id, NewRole = UserRole.Doctor };
            var result = await _service.ChangeUserRoleAsync(dto);
            Assert.True(result);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldReturnTrue_WhenUserExistsAndEmailSent()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger<AuthService>>();
            var service = new FakeAuthService(_context, configMock.Object, loggerMock.Object);

            var user = new User
            {
                Email = "test@test.com",
                PasswordHashed = "hashedpassword"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new ForgotPasswordDto { Email = user.Email };

            // Act
            var result = await service.ForgotPasswordAsync(dto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var dto = new ForgotPasswordDto { Email = "notfound@test.com" };
            var result = await _service.ForgotPasswordAsync(dto);

            Assert.False(result);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldReturnFalse_WhenEmailIsNullOrEmpty()
        {
            var dtoWithNullEmail = new ForgotPasswordDto { Email = null };
            var dtoWithEmptyEmail = new ForgotPasswordDto { Email = "" };

            var resultWithNullEmail = await _service.ForgotPasswordAsync(dtoWithNullEmail);
            var resultWithEmptyEmail = await _service.ForgotPasswordAsync(dtoWithEmptyEmail);

            Assert.False(resultWithNullEmail);
            Assert.False(resultWithEmptyEmail);
        }


        [Fact]
        public async Task FindByEmailAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { Email = "find@test.com", PasswordHashed = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.FindByEmailAsync(user.Email);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var result = await _service.ResetPasswordAsync("notfound@test.com", "token", "newpass");
            Assert.False(result);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var user = new User
            {
                Email = "ayoubdammak81@gmail.com",
                PasswordHashed = passwordHasher.HashPassword(null, "ayoub 2001"),
                ResetToken = "WwLQrpAs1bjIFwibGJcOadfcVj4O1eCOEu+ENqkbmvA=",
                ResetTokenExpiryTime = DateTime.Now.AddMinutes(10)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.ResetPasswordAsync(user.Email, "WwLQrpAs1bjIFwibGJcOadfcVj4O1eCOEu+ENqkbmvA=", "ayoub 2001");
            Assert.True(result);
        }
    }

    public class FakeAuthService : AuthService
    {
        public FakeAuthService(UserDbContext context, IConfiguration config, ILogger<AuthService> logger)
            : base(context, config, logger) { }

        protected override Task SendResetTokenEmailAsync(string email, string token)
        {
            // Simuler l’envoi de l’e-mail sans rien faire
            return Task.CompletedTask;
        }
    }

}
