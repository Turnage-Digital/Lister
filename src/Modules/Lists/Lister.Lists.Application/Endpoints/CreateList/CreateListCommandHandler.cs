using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Endpoints.CreateList;

public class CreateListCommandHandler<TList, TItem>(
    ListsAggregate<TList, TItem> listsAggregate,
    IGetListItemDefinition definitionGetter
) : IRequestHandler<CreateListCommand, ListItemDefinitionDto>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<ListItemDefinitionDto> Handle(
        CreateListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var created = await listsAggregate.CreateListAsync(
            request.UserId,
            request.Name,
            request.Statuses,
            request.Columns,
            request.Transitions,
            cancellationToken);

        var retval = await definitionGetter.GetAsync(created.Id!.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Created list definition not found");
        return retval;
    }
}
