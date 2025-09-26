using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationDeliveryAttemptDbConfiguration : IEntityTypeConfiguration<NotificationDeliveryAttemptDb>
{
    public void Configure(EntityTypeBuilder<NotificationDeliveryAttemptDb> builder)
    {
        builder.ToTable("NotificationDeliveryAttempts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ChannelJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.Property(e => e.FailureReason)
            .HasMaxLength(1000);

        builder.HasIndex(e => e.NotificationId);
        builder.HasIndex(e => e.AttemptedOn);
    }
}