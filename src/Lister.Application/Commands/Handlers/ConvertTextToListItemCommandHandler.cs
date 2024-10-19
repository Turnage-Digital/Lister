using System.Text.Json;
using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;

namespace Lister.Application.Commands.List.Handlers;

public class ConvertTextToListItemCommandHandler<TList>(
    ListAggregate<TList> listAggregate,
    IGetList<TList> listGetter,
    IGetCompletedJson completedJsonGetter,
    ILogger<ConvertTextToListItemCommandHandler<TList>> logger)
    : IRequestHandler<ConvertTextToListItemCommand, Item>
    where TList : IWritableList
{
    public async Task<Item> Handle(ConvertTextToListItemCommand request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        Item retval;

        try
        {
            var parsed = Guid.Parse(request.ListId);
            var list = await listGetter.Get(request.UserId, parsed, cancellationToken);

            if (list is null)
                throw new InvalidOperationException($"List with id {request.ListId} does not exist");

            var exampleBag = await listAggregate.CreateExampleBagAsync(list, cancellationToken);
            var exampleJson = JsonSerializer.Serialize(exampleBag);
            logger.LogInformation("Example JSON: {exampleJson}", exampleJson);
            
            var completedJson = await completedJsonGetter.Get(exampleJson, request.Text, cancellationToken);
            var completedBag = JsonSerializer.Deserialize<object>(completedJson);

            retval = new Item { Bag = completedBag ?? new object() };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to convert text to list item", ex);
        }

        return retval;
    }
}