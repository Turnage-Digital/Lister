using AutoMapper;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.CreateList;

public class CreateListCommandHandler<TList, TItem>(
    ListsAggregate<TList, TItem> listsAggregate,
    IMapper mapper
)
    : IRequestHandler<CreateListCommand, ListItemDefinition>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<ListItemDefinition> Handle(
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
            cancellationToken);

        var retval = mapper.Map<ListItemDefinition>(created);
        return retval;
    }
}