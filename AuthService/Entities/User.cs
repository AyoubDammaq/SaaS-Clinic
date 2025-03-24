namespace JWTAuthExample.Entities
{
    public class User
    {

        public Guid Id { get; set; }
        public string username { get; set; } = string.Empty;
        public string passwordHashed { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? refreshToken { get; set; }
        public DateTime? refreshTokenExpiryTime { get; set; }
    }
}
