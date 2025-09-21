using Lister.Notifications.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationRuleDbConfiguration : IEntityTypeConfiguration<NotificationRuleDb>
{
    public void Configure(EntityTypeBuilder<NotificationRuleDb> builder)
    {
        builder.ToTable("NotificationRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.ListId)
            .IsRequired();

        builder.Property(x => x.TriggerJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.Property(x => x.ChannelsJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.Property(x => x.ScheduleJson)
            .HasColumnType("JSON")
            .IsRequired();

        builder.Property(x => x.TemplateId)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(450);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ListId);
        builder.HasIndex(x => new { x.ListId, x.IsActive, x.IsDeleted });

        builder.HasMany(x => x.Notifications)
            .WithOne(x => x.NotificationRule)
            .HasForeignKey(x => x.NotificationRuleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}