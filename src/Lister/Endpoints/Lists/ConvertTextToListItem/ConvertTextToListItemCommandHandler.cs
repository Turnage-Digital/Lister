using Azure.AI.OpenAI;
using Lister.App;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Entities;
using Lister.Core.ValueObjects;
using Lister.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.ConvertTextToListItem;

public class ConvertTextToListItemCommandHandler(
    ListAggregate<ListEntity> listAggregate,
    ListerDbContext dbContext,
    IOptions<OpenAIOptions> options)
    : IRequestHandler<ConvertTextToListItemCommand, Item>
{
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<Item> Handle(ConvertTextToListItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var parsed = Guid.Parse(request.ListId);
            var list = await GetListAsync(parsed, request.UserId, cancellationToken);

            var exampleBag = await listAggregate.CreateExampleBagAsync(list, cancellationToken);
            var exampleJson = JsonConvert.SerializeObject(exampleBag);

            var completedJson = await GetCompletedJsonAsync(exampleJson, request.Text, cancellationToken);
            var completedBag = JsonConvert.DeserializeObject(completedJson);

            var retval = new Item { Bag = completedBag ?? new object() };
            return retval;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to convert text to list item", ex);
        }
    }

    private async Task<ListEntity> GetListAsync(
        Guid listId,
        string? userId,
        CancellationToken cancellationToken
    )
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == listId)
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        return retval;
    }

    private async Task<string> GetCompletedJsonAsync(string example, string text, CancellationToken cancellationToken)
    {
        var client = new OpenAIClient(_apiKey);
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage($"Here is an example of a JSON object: {example}"),
                new ChatRequestUserMessage("Can you convert the following text into that object's structure?"),
                new ChatRequestUserMessage(text)
            }
        };

        var response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
        var retval = response.Value.Choices[0].Message.Content;
        return retval;
    }
}