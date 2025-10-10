using System.Text.Json;
using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationRuleHistoryDbConfiguration : IEntityTypeConfiguration<NotificationRuleHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<NotificationRuleHistoryEntryDb> builder)
    {
        builder.ToTable("NotificationRuleHistory");

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

        builder.HasOne(e => e.NotificationRule)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.NotificationRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}