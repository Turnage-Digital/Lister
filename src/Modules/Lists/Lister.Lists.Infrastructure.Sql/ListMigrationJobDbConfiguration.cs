using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ListMigrationJobDbConfiguration : IEntityTypeConfiguration<ListMigrationJobDb>
{
    public void Configure(EntityTypeBuilder<ListMigrationJobDb> builder)
    {
        builder.ToTable("ListMigrationJobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceListId).IsRequired();
        builder.Property(x => x.RequestedBy)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.PlanJson).IsRequired();
        builder.Property(x => x.CreatedOn).IsRequired();
        builder.Property(x => x.Stage)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ListMigrationJobStage.Pending);

        builder.HasIndex(x => new { x.Stage, x.AvailableAfter });
        builder.HasIndex(x => x.CorrelationId)
            .IsUnique();
    }
}
