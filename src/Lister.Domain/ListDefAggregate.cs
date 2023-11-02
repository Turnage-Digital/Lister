using MediatR;
using Lister.Core;
using Lister.Core.ValueObjects;
using Lister.Domain.Events;

namespace Lister.Domain;

public class ListDefAggregate<TThingDef>
    where TThingDef : IWritableListDef
{
    private readonly IMediator _mediator;
    private readonly IListerUnitOfWork<TThingDef> _unitOfWork;

    public ListDefAggregate(IListerUnitOfWork<TThingDef> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<TThingDef> CreateAsync(
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
}