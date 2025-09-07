using Lister.Notifications.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Notifications.Infrastructure.Sql;

public class NotificationHistoryConfiguration : IEntityTypeConfiguration<NotificationHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<NotificationHistoryEntryDb> builder)
    {
        builder.ToTable("NotificationHistory");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(x => x.By)
            .HasMaxLength(450)
            .IsRequired();
            
        builder.Property(x => x.Bag)
            .HasColumnType("JSON");
            
        builder.HasOne(x => x.Notification)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}