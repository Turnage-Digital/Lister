using Lister.Core;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;

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

        await _mediator.Publish(new ListCreatedEvent { Id = retval.Id!.Value },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return retval;
    }
}