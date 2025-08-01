using System.Text.Json;
using Lister.Core.Domain.Services;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using MediatR;

namespace Lister.Lists.Application.Endpoints.ConvertTextToListItem;

public class ConvertTextToListItemCommandHandler<TList, TItem>(
    ListsAggregate<TList, TItem> listsAggregate,
    IGetCompletedJson completedJsonGetter,
    ILogger<ConvertTextToListItemCommandHandler<TList, TItem>> logger
)
    : IRequestHandler<ConvertTextToListItemCommand, ListItem>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<ListItem> Handle(ConvertTextToListItemCommand request, CancellationToken cancellationToken)
    {
        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetListByIdAsync(parsed, cancellationToken);
        if (list is null)
        {
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");
        }

        var exampleBag = await listsAggregate.CreateExampleBagAsync(list, cancellationToken);
        var exampleJson = JsonSerializer.Serialize(exampleBag);
        logger.LogInformation("Example JSON: {exampleJson}", exampleJson);

        var completedJson = await completedJsonGetter.Get(exampleJson, request.Text, cancellationToken);
        object? completedBag = null;
        if (completedJson is not null)
        {
            completedBag = JsonSerializer.Deserialize<object>(completedJson);
        }

        var retval = new ListItem { Bag = completedBag ?? new object() };
        return retval;
    }
}