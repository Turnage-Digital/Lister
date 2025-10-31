using Lister.Lists.Application.Mappings;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.ReadOnly.Dtos;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Commands.CreateList;

public class CreateListCommandHandler<TList, TItem>(
    ListsAggregate<TList, TItem> listsAggregate
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

        return ListDefinitionWriteContextMap.ToDto(
            created,
            await listsAggregate.GetColumnsAsync(created, cancellationToken),
            await listsAggregate.GetStatusesAsync(created, cancellationToken),
            await listsAggregate.GetStatusTransitionsAsync(created, cancellationToken));
    }
}