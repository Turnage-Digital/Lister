using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetStatusTransitions;

public class GetStatusTransitionsQueryHandler<TList, TItem>(
    IListsUnitOfWork<TList, TItem> uow
) : IRequestHandler<GetStatusTransitionsQuery, StatusTransition[]>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<StatusTransition[]> Handle(GetStatusTransitionsQuery request, CancellationToken cancellationToken)
    {
        var list = await uow.ListsStore.GetByIdAsync(request.ListId, cancellationToken)
                   ?? throw new InvalidOperationException($"List {request.ListId} not found");
        var transitions = await uow.ListsStore.GetStatusTransitionsAsync(list, cancellationToken);
        return transitions;
    }
}