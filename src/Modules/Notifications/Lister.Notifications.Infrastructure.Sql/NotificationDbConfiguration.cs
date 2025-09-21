using Lister.Notifications.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationDbConfiguration : IEntityTypeConfiguration<NotificationDb>
{
    public void Configure(EntityTypeBuilder<NotificationDb> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.ListId)
            .IsRequired();

        builder.Property(x => x.ContentJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ListId);
        builder.HasIndex(x => new { x.ProcessedOn, x.DeliveredOn });
        builder.HasIndex(x => x.CreatedOn);
        builder.HasIndex(x => new { x.UserId, x.ReadOn }); // For unread queries

        builder.HasMany(x => x.DeliveryAttempts)
            .WithOne(x => x.Notification)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}