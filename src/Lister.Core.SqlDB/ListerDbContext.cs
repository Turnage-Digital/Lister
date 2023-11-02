using Microsoft.EntityFrameworkCore;
using Lister.Core.SqlDB.Entities;

namespace Lister.Core.SqlDB;

public class ListerDbContext : DbContext
{
    public ListerDbContext(DbContextOptions<ListerDbContext> options)
        : base(options) { }

    public virtual DbSet<ListDefEntity> ThingDefs { get; set; } = null!;
}