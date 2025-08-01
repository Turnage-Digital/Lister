using AutoMapper;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.CreateListItem;

public class CreateListItemCommandHandler<TList, TItem>(ListsAggregate<TList, TItem> listsAggregate, IMapper mapper)
    : IRequestHandler<CreateListItemCommand, ListItem>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<ListItem> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetListByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        var entity = await listsAggregate.CreateItemAsync(list, request.Bag, request.UserId!, cancellationToken);
        var retval = mapper.Map<ListItem>(entity);
        return retval;
    }
}