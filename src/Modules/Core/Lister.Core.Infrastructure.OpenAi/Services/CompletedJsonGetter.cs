using Lister.Core.Domain.Services;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Lister.Core.Infrastructure.OpenAi.Services;

public class CompletedJsonGetter(IOptions<OpenAiOptions> options) : IGetCompletedJson
{
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<string?> Get(string example, string text, CancellationToken cancellationToken)
    {
        var client = new ChatClient("gpt-4.1", _apiKey);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage($"Here is an example of a JSON object: {example}"),
            new SystemChatMessage("Convert the following text into that object's structure."),
            new SystemChatMessage("Only return the JSON object, do not include any other text."),
            new UserChatMessage(text)
        };
        var response = await client.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var retval = response.Value.Content.FirstOrDefault()?.Text;
        return retval;
    }
}