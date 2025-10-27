using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class StatusTransitionDbConfiguration : IEntityTypeConfiguration<StatusTransitionDb>
{
    public void Configure(EntityTypeBuilder<StatusTransitionDb> builder)
    {
        builder.ToTable("StatusTransitions");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.From)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ListId).IsRequired();

        builder.Property(e => e.AllowedNext)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<string[]>(v, JsonSerializerOptions.Default) ?? Array.Empty<string>())
            .HasColumnType("json")
            .HasColumnName("AllowedNext");

        builder.HasIndex(e => new { e.ListId, e.From })
            .IsUnique();

        builder.HasOne(d => d.ListDb)
            .WithMany()
            .HasForeignKey(d => d.ListId);
    }
}
