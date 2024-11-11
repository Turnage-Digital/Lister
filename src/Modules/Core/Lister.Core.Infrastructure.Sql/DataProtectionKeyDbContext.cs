using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.Infrastructure.Sql;

public class DataProtectionKeyDbContext(DbContextOptions<DataProtectionKeyDbContext> options)
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
}