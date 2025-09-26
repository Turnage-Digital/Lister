using System.Text.Json;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationDbConfiguration : IEntityTypeConfiguration<NotificationDb>
{
    public void Configure(EntityTypeBuilder<NotificationDb> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ListId)
            .IsRequired();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Property(e => e.ContentJson)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<string>(e, jsonSerializerOptions)!)
            .IsRequired();

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.ListId);
        builder.HasIndex(e => new { e.ProcessedOn, e.DeliveredOn });
        builder.HasIndex(e => e.CreatedOn);
        builder.HasIndex(e => new { e.UserId, e.ReadOn }); // For unread queries

        builder.HasMany(e => e.DeliveryAttempts)
            .WithOne(e => e.Notification)
            .HasForeignKey(e => e.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}