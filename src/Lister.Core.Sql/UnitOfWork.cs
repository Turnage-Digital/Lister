using Microsoft.EntityFrameworkCore;

namespace Lister.Core.Sql;

public abstract class UnitOfWork<TContext>(TContext dbContext)
    : IUnitOfWork where TContext : DbContext
{
    private bool _disposed;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var retval = await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return retval;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken)!;
            throw;
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                dbContext.Dispose();

        _disposed = true;
    }
}