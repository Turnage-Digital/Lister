using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.UpdateListItem;

public class UpdateListItemCommandHandler<TList, TItem, TMigrationJob>(
    ListsAggregate<TList, TItem, TMigrationJob> lists
) : IRequestHandler<UpdateListItemCommand>
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    public async Task Handle(UpdateListItemCommand request, CancellationToken cancellationToken)
    {
        var list = await lists.GetListByIdAsync(request.ListId, cancellationToken)
                   ?? throw new InvalidOperationException($"List {request.ListId} not found");
        var item = await lists.GetItemByIdAsync(list, request.ItemId, cancellationToken)
                   ?? throw new InvalidOperationException($"Item {request.ItemId} not found");

        await lists.UpdateItemAsync(list, item, request.Bag, request.UserId!, cancellationToken);
    }
}