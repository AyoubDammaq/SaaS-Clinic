using Auth.Application.DTOs;
using AuthentificationService.Controllers;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using AuthentificationService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Auth.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserCreated()
        {
            var dto = new UserDto { Email = "test@test.com", Password = "Password1", ConfirmPassword = "Password1" };
            var user = new User { Email = dto.Email };
            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(user);

            var result = await _controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserExists()
        {
            var dto = new UserDto { Email = "test@test.com", Password = "Password1", ConfirmPassword = "Password1" };
            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync((User)null);

            var result = await _controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task RefreshToken_ReturnsResult_WhenValid()
        {
            var dto = new RefreshTokenRequestDto { UserId = Guid.NewGuid(), RefreshToken = "token" };
            var tokenResponse = new TokenResponseDto { AccessToken = "access", RefreshToken = "refresh" };
            _authServiceMock.Setup(s => s.RefreshTokensAsync(dto)).ReturnsAsync(tokenResponse);

            var result = await _controller.RefreshToken(dto);

            Assert.Equal(tokenResponse, result.Value);
        }

        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenInvalid()
        {
            var dto = new RefreshTokenRequestDto { UserId = Guid.NewGuid(), RefreshToken = "token" };
            _authServiceMock.Setup(s => s.RefreshTokensAsync(dto)).ReturnsAsync((TokenResponseDto)null);

            var result = await _controller.RefreshToken(dto);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            var dto = new LoginRequestDTO { Email = "test@test.com", Password = "Password1" };
            var tokenResponse = new TokenResponseDto { AccessToken = "access", RefreshToken = "refresh" };
            _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(tokenResponse);

            var result = await _controller.Login(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(tokenResponse, okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenInvalid()
        {
            var dto = new LoginRequestDTO { Email = "test@test.com", Password = "wrong" };
            _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync((TokenResponseDto)null);

            var result = await _controller.Login(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Logout_ReturnsOk_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.LogoutAsync(userId)).ReturnsAsync(true);

            var result = await _controller.Logout(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Logout successful.", okResult.Value);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenFail()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.LogoutAsync(userId)).ReturnsAsync(false);

            var result = await _controller.Logout(userId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSuccess()
        {
            var dto = new ChangePasswordDto();
            _authServiceMock.Setup(s => s.ChangePasswordAsync(dto)).ReturnsAsync(true);

            var result = await _controller.ChangePassword(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenFail()
        {
            var dto = new ChangePasswordDto();
            _authServiceMock.Setup(s => s.ChangePasswordAsync(dto)).ReturnsAsync(false);

            var result = await _controller.ChangePassword(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithUsers()
        {
            var users = new List<User> { new User { Email = "a@a.com" } };
            _authServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetUserDetailsById_ReturnsOk_WhenFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            _authServiceMock.Setup(s => s.GetUserDetailsByIdAsync(userId)).ReturnsAsync(user);

            var result = await _controller.GetUserDetailsById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUserDetailsById_ReturnsNotFound_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.GetUserDetailsByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetUserDetailsById(userId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(true);

            var result = await _controller.DeleteUser(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest_WhenFail()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(false);

            var result = await _controller.DeleteUser(userId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsOk_WhenSuccess()
        {
            var dto = new ChangeUserRoleRequestDto();
            _authServiceMock.Setup(s => s.ChangeUserRoleAsync(dto)).ReturnsAsync(true);

            var result = await _controller.ChangeUserRole(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Role changed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsBadRequest_WhenFail()
        {
            var dto = new ChangeUserRoleRequestDto();
            _authServiceMock.Setup(s => s.ChangeUserRoleAsync(dto)).ReturnsAsync(false);

            var result = await _controller.ChangeUserRole(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOk_WhenSuccess()
        {
            var dto = new ForgotPasswordDto();
            _authServiceMock.Setup(s => s.ForgotPasswordAsync(dto)).ReturnsAsync(true);

            var result = await _controller.ForgotPassword(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("successful", okResult.Value.ToString());
        }

        [Fact]
        public async Task ForgotPassword_ReturnsBadRequest_WhenFail()
        {
            var dto = new ForgotPasswordDto();
            _authServiceMock.Setup(s => s.ForgotPasswordAsync(dto)).ReturnsAsync(false);

            var result = await _controller.ForgotPassword(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenSuccess()
        {
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "token", NewPassword = "Password1" };
            var user = new User { Email = dto.Email };
            _authServiceMock.Setup(s => s.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _authServiceMock.Setup(s => s.ResetPasswordAsync(dto.Email, It.IsAny<string>(), dto.NewPassword)).ReturnsAsync(true);

            var result = await _controller.ResetPassword(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("reset successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task ResetPassword_ReturnsNotFound_WhenUserNotFound()
        {
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "token", NewPassword = "Password1" };
            _authServiceMock.Setup(s => s.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null);

            var result = await _controller.ResetPassword(dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenResetFails()
        {
            var dto = new ResetPasswordDto { Email = "test@test.com", Token = "token", NewPassword = "Password1" };
            var user = new User { Email = dto.Email };
            _authServiceMock.Setup(s => s.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _authServiceMock.Setup(s => s.ResetPasswordAsync(dto.Email, It.IsAny<string>(), dto.NewPassword)).ReturnsAsync(false);

            var result = await _controller.ResetPassword(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
