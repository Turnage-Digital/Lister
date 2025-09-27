using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
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

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Required)
            .HasDefaultValue(false)
            .IsRequired();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Property(e => e.AllowedValues)
            .HasColumnType("JSON")
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<string[]?>(v, jsonSerializerOptions)!)
            .IsRequired(false);

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

        builder.HasOne(d => d.ListDb)
            .WithMany(p => p.Columns)
            .HasForeignKey(d => d.ListId);
    }
}