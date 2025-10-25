using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.ValueObjects;
using MediatR;

namespace Lister.Lists.Application.Endpoints.GetStatusTransitions;

public class GetStatusTransitionsQueryHandler(
    IGetListItemDefinition definitionGetter
) : IRequestHandler<GetStatusTransitionsQuery, StatusTransition[]>
{
    public async Task<StatusTransition[]> Handle(GetStatusTransitionsQuery request, CancellationToken cancellationToken)
    {
        var def = await definitionGetter.GetAsync(request.ListId, cancellationToken)
                  ?? throw new InvalidOperationException($"List {request.ListId} not found");
        return def.Transitions;
    }
}