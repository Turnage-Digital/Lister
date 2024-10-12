using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Core.Sql.Entities;
using Lister.Core.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.Sql;

public class ListerDbContext(DbContextOptions<ListerDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<ListDb> Lists { get; set; } = null!;

    public virtual DbSet<ItemDb> Items { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ListDb>(entity =>
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

            entity.Property(e => e.DeletedBy)
                .HasMaxLength(50);

            entity.Property(e => e.DeletedOn);

            entity.HasMany(e => e.Columns)
                .WithOne(d => d.ListDb)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Statuses)
                .WithOne(d => d.ListDb)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Items)
                .WithOne(e => e.ListDb)
                .HasForeignKey(e => e.ListId);
        });

        modelBuilder.Entity<ColumnDb>(entity =>
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

            entity.HasOne(d => d.ListDb)
                .WithMany(p => p.Columns)
                .HasForeignKey(d => d.ListId);
        });

        modelBuilder.Entity<StatusDb>(entity =>
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

            entity.HasOne(e => e.ListDb)
                .WithMany(e => e.Statuses)
                .HasForeignKey(e => e.ListId);
        });

        modelBuilder.Entity<ItemDb>(entity =>
        {
            entity.ToTable("Items");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            entity.Property(e => e.Bag)
                .HasColumnType("JSON")
                .HasConversion(
                    e => JsonSerializer.Serialize(e, jsonSerializerOptions),
                    e => JsonSerializer.Deserialize<object>(e, jsonSerializerOptions)!)
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedOn)
                .IsRequired();

            entity.Property(e => e.DeletedBy)
                .HasMaxLength(50);

            entity.Property(e => e.DeletedOn);

            entity.Property(e => e.ListId)
                .HasColumnName("ListId");

            entity.HasOne(d => d.ListDb)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.ListId);
        });
    }
}