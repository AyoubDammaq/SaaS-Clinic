using JWTAuthExample.Entities;
using JWTAuthExample.Models;
using JWTAuthExample.Services;
using Microsoft.AspNetCore.Mvc;


namespace JWTAuthExample.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        public static User user = new();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register (UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if(user is null)
            {
                return BadRequest("Username already exists.");
            }
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if(result is null || result.refreshToken is null || result.accessToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            return result;
        }

        
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {

            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password");
            }
            
            return Ok(result);
        }

    }
}
