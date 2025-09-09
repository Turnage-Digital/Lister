using System.ComponentModel.DataAnnotations.Schema;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class StatusDbConfiguration : IEntityTypeConfiguration<StatusDb>
{
    public void Configure(EntityTypeBuilder<StatusDb> builder)
    {
        builder.ToTable("Statuses");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Color)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ListId);

        builder.HasOne(e => e.ListDb)
            .WithMany(e => e.Statuses)
            .HasForeignKey(e => e.ListId);
    }
}