using Lister.Core;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;
using Serilog;

namespace Lister.Domain;

public class ListAggregate<TList>
    where TList : IWritableList
{
    private readonly IMediator _mediator;
    private readonly IListerUnitOfWork<TList> _unitOfWork;

    public ListAggregate(IListerUnitOfWork<TList> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<TList?> ReadAsync(string id, CancellationToken cancellationToken = default)
    {
        var retval = await _unitOfWork.ListsStore.ReadAsync(id, cancellationToken);
        return retval;
    }

    public async Task<TList> CreateAsync(
        string createdBy,
        string name,
        IEnumerable<Status> statuses,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _unitOfWork.ListsStore.InitAsync(createdBy, name, cancellationToken);
        await _unitOfWork.ListsStore.SetColumnsAsync(retval, columns, cancellationToken);
        await _unitOfWork.ListsStore.SetStatusesAsync(retval, statuses, cancellationToken);
        await _unitOfWork.ListsStore.CreateAsync(retval, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var id = retval.GetId();
        Log.Information("Created list: {id}", id);

        await _mediator.Publish(new ListCreatedEvent(id), cancellationToken);
        return retval;
    }

    public async Task<Item> CreateItemAsync(TList list, object bag, CancellationToken cancellationToken = default)
    {
        var retval = await _unitOfWork.ListsStore.InitItemAsync(list, bag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var id = retval.GetId();
        Log.Information("Created list item: {id}", id);
        
        await _mediator.Publish(new ListItemCreatedEvent(id), cancellationToken);
        return retval;
    }
}