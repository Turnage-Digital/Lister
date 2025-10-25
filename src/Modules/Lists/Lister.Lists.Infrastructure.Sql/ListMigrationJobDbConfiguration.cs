using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ListMigrationJobDbConfiguration : IEntityTypeConfiguration<ListMigrationJobDb>
{
    public void Configure(EntityTypeBuilder<ListMigrationJobDb> builder)
    {
        builder.ToTable("ListMigrationJobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(j => j.RequestedByUserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(j => j.PlanJson)
            .IsRequired();

        builder.Property(j => j.CurrentMessage)
            .HasMaxLength(256);

        builder.Property(j => j.FailureReason)
            .HasMaxLength(1024);

        builder.HasIndex(j => new { j.ListId, j.Status })
            .HasDatabaseName("IX_ListMigrationJobs_ListId_Status");

        builder.HasIndex(j => j.Status)
            .HasDatabaseName("IX_ListMigrationJobs_Status");

        builder.Navigation(j => j.History)
            .AutoInclude(false);
    }
}