using Lister.Lists.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsDbContext(DbContextOptions<ListsDbContext> options)
    : DbContext(options)
{
    public virtual DbSet<ListDb> Lists { get; set; } = null!;

    public virtual DbSet<ItemDb> Items { get; set; } = null!;

    public virtual DbSet<ListMigrationJobDb> ListMigrationJobs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListsDbContext).Assembly);
    }
}
