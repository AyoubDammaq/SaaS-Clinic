using Auth.Application.DTOs;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using AuthentificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web;


namespace AuthentificationService.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register ([FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(request);
            if(user is null)
            {
                return BadRequest("Username already exists.");
            }
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokensAsync(request);
            if(result is null || result.RefreshToken is null || result.AccessToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            return result;
        }

        
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDTO request)
        {

            var result = await _authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password");
            }
            
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            _logger.LogInformation("Logout called with: {@Request}", request);

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        _logger.LogWarning("Validation error for {Key}: {Error}", kvp.Key, error.ErrorMessage);
                    }
                }

                return BadRequest(ModelState);
            }

            var result = await _authService.LogoutAsync(request.Email);

            if (!result)
            {
                _logger.LogError("Logout failed for user {UserId}", request.Email );
                return BadRequest("Logout failed.");
            }

            _logger.LogInformation("Logout successful for user {UserId}", request.Email);
            return Ok("Logout successful.");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var result = await _authService.ChangePasswordAsync(request);
            if (!result)
            {
                return BadRequest("Password change failed.");
            }
            return Ok("Password changed successfully.");
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUserDetailsById([FromRoute] Guid id)
        {
            var user = await _authService.GetUserDetailsByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var result = await _authService.DeleteUserAsync(userId);
            if (!result)
            {
                return BadRequest("User deletion failed.");
            }
            return Ok("User deleted successfully.");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequestDto request)
        {
            var result = await _authService.ChangeUserRoleAsync(request);
            if (!result)
            {
                return BadRequest("Role change failed.");
            }
            return Ok("Role changed successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _authService.ForgotPasswordAsync(request);
            if (!result)
            {
                return BadRequest("Password reset request failed.");
            }
            return Ok("Password reset request successful. Please check your email for further instructions.");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState invalide : {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Réinitialisation demandée via API pour l'email : {Email}, token : {Token}", model.Email, model.Token);

            var user = await _authService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Aucun utilisateur trouvé avec l'email : {Email}", model.Email);
                return NotFound("User not found.");
            }

            var resetResult = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (!resetResult)
            {
                _logger.LogWarning("Échec de la réinitialisation du mot de passe pour l'email : {Email}", model.Email);
                return BadRequest("Password reset failed.");
            }

            return Ok(new { message = "Password has been reset successfully." });
        }

    }
}
