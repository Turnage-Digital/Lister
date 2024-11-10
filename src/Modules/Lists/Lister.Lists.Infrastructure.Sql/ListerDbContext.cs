using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

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

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasMany(e => e.Columns)
                .WithOne(d => d.ListDb)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Statuses)
                .WithOne(d => d.ListDb)
                .HasForeignKey(d => d.ListId);

            entity.HasMany(e => e.Items)
                .WithOne(e => e.List)
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

            entity.Property(e => e.ListId);

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

            entity.Property(e => e.ListId);

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

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.ListId);

            entity.HasOne(d => d.List)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.ListId);
        });

        modelBuilder.Entity<ItemHistoryEntryDb>(entity =>
        {
            entity.ToTable("ItemHistory");

            entity.HasKey(e => e.Id)
                .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

            entity.Property(e => e.Type)
                .IsRequired();

            entity.Property(e => e.On)
                .IsRequired();

            entity.Property(e => e.By)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ItemId)
                .IsRequired();

            entity.HasOne(e => e.Item)
                .WithMany(e => e.History)
                .HasForeignKey(e => e.ItemId);
        });
    }
}