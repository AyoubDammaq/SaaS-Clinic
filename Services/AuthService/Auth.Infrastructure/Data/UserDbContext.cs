using AuthentificationService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthentificationService.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users{ get; set; }
    }
}
