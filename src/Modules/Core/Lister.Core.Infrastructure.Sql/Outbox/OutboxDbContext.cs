using Microsoft.EntityFrameworkCore;

namespace Lister.Core.Infrastructure.Sql.Outbox;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options)
    : DbContext(options)
{
    public DbSet<OutboxMessageDb> Messages => Set<OutboxMessageDb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entity = modelBuilder.Entity<OutboxMessageDb>();
        entity.ToTable("OutboxMessages");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Type).HasMaxLength(300).IsRequired();
        entity.Property(x => x.PayloadJson).HasColumnType("LONGTEXT").IsRequired();
        entity.Property(x => x.CreatedOn).IsRequired();
        entity.Property(x => x.Attempts).HasDefaultValue(0).IsRequired();
        entity.Property(x => x.LastError).HasMaxLength(2000);
        entity.HasIndex(x => x.ProcessedOn);
        entity.HasIndex(x => new { x.CreatedOn, x.ProcessedOn });
    }
}