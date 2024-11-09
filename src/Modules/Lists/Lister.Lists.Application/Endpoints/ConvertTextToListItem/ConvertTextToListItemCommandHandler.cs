using System.Text.Json;
using Lister.Core.Domain.Services;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Endpoints.ConvertTextToListItem;

public class ConvertTextToListItemCommandHandler<TList>(
    ListsAggregate<TList> listsAggregate,
    IGetCompletedJson completedJsonGetter,
    ILogger<ConvertTextToListItemCommandHandler<TList>> logger)
    : IRequestHandler<ConvertTextToListItemCommand, Item>
    where TList : IWritableList
{
    public async Task<Item> Handle(ConvertTextToListItemCommand request, CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        var parsed = Guid.Parse(request.ListId);
        var list = await listsAggregate.GetByIdAsync(parsed, cancellationToken);
        if (list is null)
            throw new InvalidOperationException($"List with id {request.ListId} does not exist");

        var exampleBag = await listsAggregate.CreateExampleBagAsync(list, cancellationToken);
        var exampleJson = JsonSerializer.Serialize(exampleBag);
        logger.LogInformation("Example JSON: {exampleJson}", exampleJson);

        var completedJson = await completedJsonGetter.Get(exampleJson, request.Text, cancellationToken);
        var completedBag = JsonSerializer.Deserialize<object>(completedJson);

        var retval = new Item { Bag = completedBag ?? new object() };
        return retval;
    }
}