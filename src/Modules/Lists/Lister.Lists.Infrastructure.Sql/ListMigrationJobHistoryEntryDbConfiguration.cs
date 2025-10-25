using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ListMigrationJobHistoryEntryDbConfiguration : IEntityTypeConfiguration<ListMigrationJobHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<ListMigrationJobHistoryEntryDb> builder)
    {
        builder.ToTable("ListMigrationJobHistory");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.On)
            .IsRequired();

        builder.Property(e => e.By)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.MigrationJobId)
            .IsRequired();

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        builder.Property(e => e.Bag)
            .HasColumnType("JSON")
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<object>(v, jsonOptions)!);

        builder.HasOne(e => e.MigrationJob)
            .WithMany(j => j.History)
            .HasForeignKey(e => e.MigrationJobId);
    }
}