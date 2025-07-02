using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Notif.Domain.Entities;
using Notif.Domain.Enums;
using Notif.Domain.ValueObject;
using System.Text.Json;

namespace Notif.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotificationPreferences> UserPreferences { get; set; }


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

            modelBuilder.Entity<UserNotificationPreferences>(entity =>
            {
                entity.HasKey(p => p.UserId);

                entity.Property(p => p.UserType)
                      .HasConversion<string>();

                entity.Property(p => p.PreferredChannels)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(e => Enum.Parse<NotificationChannel>(e)).ToList()
                      )
                      .Metadata.SetValueComparer(
                          new ValueComparer<List<NotificationChannel>>(
                              (c1, c2) => c1.SequenceEqual(c2),
                              c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                              c => c.ToList()
                          )
                      );

                entity.Property(p => p.NotificationSettings)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<Dictionary<NotificationType, bool>>(v, (JsonSerializerOptions)null)
                      )
                      .Metadata.SetValueComparer(
                          new ValueComparer<Dictionary<NotificationType, bool>>(
                              (d1, d2) => JsonSerializer.Serialize(d1, (JsonSerializerOptions)null) == JsonSerializer.Serialize(d2, (JsonSerializerOptions)null),
                              d => JsonSerializer.Serialize(d, (JsonSerializerOptions)null).GetHashCode(),
                              d => JsonSerializer.Deserialize<Dictionary<NotificationType, bool>>(JsonSerializer.Serialize(d, (JsonSerializerOptions)null), (JsonSerializerOptions)null)
                          )
                      );

                entity.Property(p => p.Language).HasMaxLength(5).HasDefaultValue("fr");
                entity.Property(p => p.PhoneNumber).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(255);
            });
        }
    }
}
