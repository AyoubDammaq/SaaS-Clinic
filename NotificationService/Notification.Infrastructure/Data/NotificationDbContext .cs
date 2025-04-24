using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Domain.Entities.Notification> Notifications { get; set; }
        public DbSet<PreferenceNotification> Preferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Notification>()
                .Property(n => n.Canaux)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => Enum.Parse<CanalNotification>(c)).ToList()
                );

            modelBuilder.Entity<Domain.Entities.Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Domain.Entities.Notification>()
                .Property(n => n.Statut)
                .HasConversion<string>();

            modelBuilder.Entity<PreferenceNotification>()
                .HasKey(p => p.UtilisateurId);
        }
    }
}
