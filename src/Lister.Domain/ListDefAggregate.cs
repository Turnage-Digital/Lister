using Lister.Core;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;
using MediatR;

namespace Lister.Domain;

public class ListDefAggregate<TListDef>
    where TListDef : IWritableListDef
{
    private readonly IMediator _mediator;
    private readonly IListerUnitOfWork<TListDef> _unitOfWork;

    public ListDefAggregate(IListerUnitOfWork<TListDef> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<TListDef> CreateAsync(
        string createdBy,
        string name,
        StatusDef[] statusDefs,
        ColumnDef[] propertyDefs,
        CancellationToken cancellationToken = default
    )
    {
        var retval = await _unitOfWork.ListDefsStore.InitAsync(cancellationToken);

        await _unitOfWork.ListDefsStore.SetCreatedByAsync(retval, createdBy, cancellationToken);
        await _unitOfWork.ListDefsStore.SetNameAsync(retval, name, cancellationToken);
        await _unitOfWork.ListDefsStore.SetStatusDefsAsync(retval, statusDefs, cancellationToken);
        await _unitOfWork.ListDefsStore.SetColumnDefsAsync(retval, propertyDefs, cancellationToken);

        await _unitOfWork.ListDefsStore.CreateAsync(retval, cancellationToken);

        await _mediator.Publish(new ListDefCreatedEvent { Id = retval.Id!.Value },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return retval;
    }

    public async Task UpdateAsync(
        string id,
        string name,
        StatusDef[] statusDefs,
        ColumnDef[] columnDefs,
        CancellationToken cancellationToken = default
    )
    {
        var listDef = await _unitOfWork.ListDefsStore.ReadAsync(id, cancellationToken);
        if (listDef == null)
        {
            throw new InvalidOperationException($"ListDef with id {id} not found.");
        }

        await _unitOfWork.ListDefsStore.SetNameAsync(listDef, name, cancellationToken);
        await _unitOfWork.ListDefsStore.SetStatusDefsAsync(listDef, statusDefs, cancellationToken);
        await _unitOfWork.ListDefsStore.SetColumnDefsAsync(listDef, columnDefs, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}