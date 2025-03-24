namespace JWTAuthExample.Models
{
    public class TokenResponseDto
    {
        public required string accessToken { get; set; }
        public required string refreshToken { get; set; }
    }
}
