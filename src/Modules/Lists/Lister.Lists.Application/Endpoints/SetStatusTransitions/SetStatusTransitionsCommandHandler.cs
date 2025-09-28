using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.SetStatusTransitions;

public class SetStatusTransitionsCommandHandler<TList, TItem>(
    ListsAggregate<TList, TItem> aggregate
) : IRequestHandler<SetStatusTransitionsCommand>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task Handle(SetStatusTransitionsCommand request, CancellationToken cancellationToken)
    {
        var list = await aggregate.GetListByIdAsync(request.ListId, cancellationToken)
                   ?? throw new InvalidOperationException($"List {request.ListId} not found");

        await aggregate.UpdateListAsync(
            list,
            null,
            null,
            request.Transitions,
            "system",
            cancellationToken);
    }
}