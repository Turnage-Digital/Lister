using System.ComponentModel.DataAnnotations.Schema;
using Lister.Core.SqlDB.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Lister.Core.SqlDB;

public class ListerDbContext(DbContextOptions<ListerDbContext> options) : DbContext(options)
{
    public virtual DbSet<ListEntity> Lists { get; set; } = null!;

    public virtual DbSet<ItemEntity> Items { get; set; } = null!;

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
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedOn)
                .IsRequired();

            entity.HasMany(e => e.Columns)
                .WithOne(d => d.List)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Statuses)
                .WithOne(d => d.List)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Items)
                .WithOne(e => e.List)
                .HasForeignKey(e => e.ListId);
        });

        modelBuilder.Entity<ColumnEntity>(entity =>
        {
            entity.ToTable("Columns");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Type)
                .IsRequired();

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

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ListId)
                .HasColumnName("ListId");

            entity.HasOne(e => e.List)
                .WithMany(e => e.Statuses)
                .HasForeignKey(e => e.ListId);
        });

        modelBuilder.Entity<ItemEntity>(entity =>
        {
            entity.ToTable("Items");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.Bag)
                .HasColumnType("JSON")
                .HasConversion(
                    e => JsonConvert.SerializeObject(e),
                    e => JsonConvert.DeserializeObject<object>(e)!)
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedOn)
                .IsRequired();

            entity.Property(e => e.ListId)
                .HasColumnName("ListId");

            entity.HasOne(d => d.List)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.ListId);
        });
    }
}