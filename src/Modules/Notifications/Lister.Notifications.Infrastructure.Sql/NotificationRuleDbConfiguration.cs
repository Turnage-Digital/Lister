using System.Text.Json;
using Lister.Notifications.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationRuleDbConfiguration : IEntityTypeConfiguration<NotificationRuleDb>
{
    public void Configure(EntityTypeBuilder<NotificationRuleDb> builder)
    {
        builder.ToTable("NotificationRules");

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

        builder.Property(e => e.TriggerJson)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<string>(e, jsonSerializerOptions)!)
            .IsRequired();

        builder.Property(e => e.ChannelsJson)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<string>(e, jsonSerializerOptions)!)
            .IsRequired();

        builder.Property(e => e.ScheduleJson)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<string>(e, jsonSerializerOptions)!)
            .IsRequired();

        builder.Property(e => e.TriggerType)
            .IsRequired();

        builder.Property(e => e.TemplateId)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(450);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.ListId);
        builder.HasIndex(e => new { e.ListId, e.IsActive, e.IsDeleted });
        builder.HasIndex(e => new { e.ListId, e.TriggerType });

        builder.HasMany(e => e.Notifications)
            .WithOne(e => e.NotificationRule)
            .HasForeignKey(e => e.NotificationRuleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}