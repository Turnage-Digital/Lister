using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ItemDbConfiguration : IEntityTypeConfiguration<ItemDb>
{
    public void Configure(EntityTypeBuilder<ItemDb> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Property(e => e.Bag)
            .HasColumnType("JSON")
            .HasConversion(
                e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                e => JsonSerializer.Deserialize<object>(e, jsonSerializerOptions)!)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ListId);

        builder.HasOne(d => d.List)
            .WithMany(p => p.Items)
            .HasForeignKey(d => d.ListId);
    }
}