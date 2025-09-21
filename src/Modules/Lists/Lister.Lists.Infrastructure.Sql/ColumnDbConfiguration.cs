using System.ComponentModel.DataAnnotations.Schema;
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

        builder.Property(e => e.ListId);

        builder.HasOne(d => d.ListDb)
            .WithMany(p => p.Columns)
            .HasForeignKey(d => d.ListId);
    }
}