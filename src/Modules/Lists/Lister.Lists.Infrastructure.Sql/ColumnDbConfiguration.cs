using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ColumnDbConfiguration : IEntityTypeConfiguration<ColumnDb>
{
    public void Configure(EntityTypeBuilder<ColumnDb> builder)
    {
        builder.ToTable("Columns");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

        // StorageKey is introduced for stable property addressing during migrations.
        // Not persisted yet to avoid forcing a DB migration in this change set.
        builder.Ignore(e => e.StorageKey);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Required)
            .HasDefaultValue(false)
            .IsRequired();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var comparer = new ValueComparer<string[]?>(
            (a, b) => ReferenceEquals(a, b) || (a != null && b != null && a.SequenceEqual(b)),
            v => v == null ? 0 : v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
            v => v == null ? null : v.ToArray()
        );

        var allowedValues = builder.Property(e => e.AllowedValues)
            .HasColumnType("JSON")
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<string[]?>(v, jsonSerializerOptions)!)
            .IsRequired(false);
        allowedValues.Metadata.SetValueComparer(comparer);

        builder.Property(e => e.MinNumber)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired(false);

        builder.Property(e => e.MaxNumber)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired(false);

        builder.Property(e => e.Regex)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(e => e.ListId);

        builder.HasOne(e => e.ListDb)
            .WithMany(e => e.Columns)
            .HasForeignKey(e => e.ListId);
    }
}