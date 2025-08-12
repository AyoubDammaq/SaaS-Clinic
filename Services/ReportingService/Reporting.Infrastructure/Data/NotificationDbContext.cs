using Microsoft.EntityFrameworkCore;
using Notif.Domain.Entities;

namespace Notif.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.RecipientId).IsRequired();
                entity.Property(n => n.RecipientType)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(n => n.Type)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(n => n.Channel)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(n => n.Title)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(n => n.Content)
                      .IsRequired();

                entity.Property(n => n.Priority)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(n => n.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(n => n.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(n => n.ScheduledAt);
                entity.Property(n => n.SentAt);

                entity.Property(n => n.RetryCount)
                      .HasDefaultValue(0);

                entity.Property(n => n.ErrorMessage)
                      .HasMaxLength(1000);
            });
        }
    }
}
