using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.ReadOnly.Queries;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetStatusTransitions;

public class GetStatusTransitionsQueryHandler<TList, TItem>(
    IGetListItemDefinition definitionGetter
) : IRequestHandler<GetStatusTransitionsQuery, StatusTransition[]>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<StatusTransition[]> Handle(GetStatusTransitionsQuery request, CancellationToken cancellationToken)
    {
        var def = await definitionGetter.GetAsync(request.ListId, cancellationToken)
                  ?? throw new InvalidOperationException($"List {request.ListId} not found");

        return def.Transitions
            .Select(t => new StatusTransition
            {
                From = t.From,
                AllowedNext = t.AllowedNext
            })
            .ToArray();
    }
}