using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationRuleHistoryDbConfiguration : IEntityTypeConfiguration<NotificationRuleHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<NotificationRuleHistoryEntryDb> builder)
    {
        builder.ToTable("NotificationRuleHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.By)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Bag)
            .HasColumnType("JSON");

        builder.HasOne(x => x.NotificationRule)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.NotificationRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}