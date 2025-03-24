using JWTAuthExample.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthExample.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users{ get; set; }
    }
}
