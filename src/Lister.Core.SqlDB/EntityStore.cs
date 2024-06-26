using Microsoft.EntityFrameworkCore;

namespace Lister.Core.SqlDB;

internal class EntityStore<TEntity>(DbContext dbContext)
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public ValueTask<TEntity?> ReadAsync(object id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(new[] { id }, cancellationToken);
    }
}