using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ItemHistoryEntryDbConfiguration : IEntityTypeConfiguration<ItemHistoryEntryDb>
{
    public void Configure(EntityTypeBuilder<ItemHistoryEntryDb> builder)
    {
        builder.ToTable("ItemHistory");

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

        builder.Property(e => e.ItemId)
            .IsRequired();

        builder.HasOne(e => e.Item)
            .WithMany(e => e.History)
            .HasForeignKey(e => e.ItemId);
    }
}