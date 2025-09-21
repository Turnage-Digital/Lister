using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationDeliveryAttemptDbConfiguration : IEntityTypeConfiguration<NotificationDeliveryAttemptDb>
{
    public void Configure(EntityTypeBuilder<NotificationDeliveryAttemptDb> builder)
    {
        builder.ToTable("NotificationDeliveryAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ChannelJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.NotificationId);
        builder.HasIndex(x => x.AttemptedOn);
    }
}