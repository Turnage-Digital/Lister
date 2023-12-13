using System.ComponentModel.DataAnnotations.Schema;
using Lister.Core.SqlDB.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

public class ListerDbContext : DbContext
{
    public ListerDbContext(DbContextOptions<ListerDbContext> options)
        : base(options) { }

    public virtual DbSet<ListEntity> Lists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ListEntity>(entity =>
        {
            entity.ToTable("Lists");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.CreatedOn)
                .IsRequired();

            entity.HasMany(e => e.Columns)
                .WithOne(d => d.List)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Statuses)
                .WithOne(d => d.List)
                .HasForeignKey(d => d.ListId);
        });

        modelBuilder.Entity<ColumnEntity>(entity =>
        {
            entity.ToTable("Columns");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.ListId)
                .HasColumnName("ListId");

            entity.HasOne(d => d.List)
                .WithMany(p => p.Columns)
                .HasForeignKey(d => d.ListId);
        });

        modelBuilder.Entity<StatusEntity>(entity =>
        {
            entity.ToTable("Statuses");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.ListId)
                .HasColumnName("ListId");

            entity.HasOne(d => d.List)
                .WithMany(p => p.Statuses)
                .HasForeignKey(d => d.ListId);
        });
    }
}