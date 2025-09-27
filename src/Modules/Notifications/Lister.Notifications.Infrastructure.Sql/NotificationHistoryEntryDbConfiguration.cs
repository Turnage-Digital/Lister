using System.Text.Json;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationHistoryEntryDbConfiguration : IEntityTypeConfiguration<NotificationHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<NotificationHistoryEntryDb> builder)
    {
        builder.ToTable("NotificationHistory");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.By)
            .HasMaxLength(450)
            .IsRequired();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Property(e => e.Bag)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<Dictionary<string, object?>>(e, jsonSerializerOptions)!);

        builder.HasOne(e => e.Notification)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}