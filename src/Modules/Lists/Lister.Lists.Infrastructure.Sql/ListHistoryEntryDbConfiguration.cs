using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ListHistoryEntryDbConfiguration : IEntityTypeConfiguration<ListHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<ListHistoryEntryDb> builder)
    {
        builder.ToTable("ListHistory");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.On)
            .IsRequired();

        builder.Property(e => e.By)
            .HasMaxLength(50)
            .IsRequired();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Property(e => e.Bag)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<object>(e, jsonSerializerOptions)!);

        builder.Property(e => e.ListId)
            .IsRequired();

        builder.HasOne(e => e.List)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.ListId);
    }
}