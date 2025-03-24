namespace JWTAuthExample.Models
{
    public class RefreshTokenRequestDto
    { 
        public Guid UserId{ get; set; }
        public required string refreshToken{ get; set; }
    }
}
