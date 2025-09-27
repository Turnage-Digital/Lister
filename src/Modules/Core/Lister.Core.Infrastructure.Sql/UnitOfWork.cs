using Lister.Core.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Core.Infrastructure.Sql;

public abstract class UnitOfWork<TContext>(TContext dbContext, IMediator mediator, IDomainEventQueue eventQueue)
    : IUnitOfWork where TContext : DbContext
{
    private readonly IDomainEventQueue _eventQueue = eventQueue;
    private readonly IMediator _mediator = mediator;
    private bool _disposed;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        // Publish any queued BeforeSave events
        foreach (var e in _eventQueue.Dequeue(EventPhase.BeforeSave))
        {
            await _mediator.Publish(e, cancellationToken);
        }

        // Only wrap in a transaction for relational providers
        if (dbContext.Database.IsRelational())
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var retval = await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                foreach (var e in _eventQueue.Dequeue(EventPhase.AfterSave))
                {
                    await _mediator.Publish(e, cancellationToken);
                }

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
        else
        {
            // InMemory and other non-relational providers: no transaction
            var retval = await dbContext.SaveChangesAsync(cancellationToken);

            foreach (var e in _eventQueue.Dequeue(EventPhase.AfterSave))
            {
                await _mediator.Publish(e, cancellationToken);
            }

            return retval;
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
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }

        _disposed = true;
    }
}
