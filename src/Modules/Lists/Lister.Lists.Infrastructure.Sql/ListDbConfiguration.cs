using System.ComponentModel.DataAnnotations.Schema;
using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lister.Lists.Infrastructure.Sql;

public class ListDbConfiguration : IEntityTypeConfiguration<ListDb>
{
    public void Configure(EntityTypeBuilder<ListDb> builder)
    {
        builder.ToTable("Lists");

        builder.HasKey(e => e.Id)
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(e => e.Columns)
            .WithOne(d => d.ListDb)
            .HasForeignKey(d => d.ListId);

        builder.HasMany(e => e.Statuses)
            .WithOne(d => d.ListDb)
            .HasForeignKey(d => d.ListId);

        builder.HasMany(e => e.Items)
            .WithOne(e => e.List)
            .HasForeignKey(e => e.ListId);

        builder.HasMany(typeof(Lister.Lists.Infrastructure.Sql.ValueObjects.StatusTransitionDb), nameof(ListDb.StatusTransitions))
            .WithOne("ListDb")
            .HasForeignKey("ListId");
    }
}
