using System.Text.Json;
using Azure.AI.OpenAI;
using Lister.Domain;
using Lister.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Lister.Application.Commands;

public class ConvertTextToListItemCommandHandler<TList, TItem>(
    ListAggregate<TList, TItem> listAggregate,
    IGetList<TList> listGetter,
    IOptions<OpenAIOptions> options,
    ILogger<ConvertTextToListItemCommandHandler<TList, TItem>> logger)
    : IRequestHandler<ConvertTextToListItemCommand, Item>
    where TList : IWritableList where TItem : Item
{
    private readonly string _apiKey = options.Value.ApiKey;

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

            var completedJson = await GetCompletedJsonAsync(exampleJson, request.Text, cancellationToken);
            var completedBag = JsonSerializer.Deserialize<object>(completedJson);

            retval = new Item { Bag = completedBag ?? new object() };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to convert text to list item", ex);
        }

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