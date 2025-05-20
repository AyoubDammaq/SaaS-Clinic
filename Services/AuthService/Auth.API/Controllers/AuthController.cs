using Auth.Application.DTOs;
using AuthentificationService.Entities;
using AuthentificationService.Models;
using AuthentificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;


namespace AuthentificationService.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<IActionResult> Logout([FromBody] Guid userId)
        {
            var result = await _authService.LogoutAsync(userId);
            if (!result)
            {
                return BadRequest("Logout failed.");
            }
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

        //[Authorize(Roles = "SuperAdmin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUserDetailsById([FromRoute] Guid userId)
        {
            var user = await _authService.GetUserDetailsByIdAsync(userId);
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
                return BadRequest(ModelState);

            var user = await _authService.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound("User not found.");

            var decodedToken = HttpUtility.UrlDecode(model.Token);

            var resetResult = await _authService.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (!resetResult)
            {
                return BadRequest("Password reset failed.");
            }

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
