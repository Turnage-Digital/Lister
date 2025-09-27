using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.SetStatusTransitions;

public class SetStatusTransitionsCommandHandler<TList, TItem>(
    IListsUnitOfWork<TList, TItem> uow
) : IRequestHandler<SetStatusTransitionsCommand>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task Handle(SetStatusTransitionsCommand request, CancellationToken cancellationToken)
    {
        var list = await uow.ListsStore.GetByIdAsync(request.ListId, cancellationToken)
                   ?? throw new InvalidOperationException($"List {request.ListId} not found");
        await uow.ListsStore.SetStatusTransitionsAsync(list, request.Transitions, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
    }
}

