using Azure.AI.OpenAI;
using Lister.Domain;
using Microsoft.Extensions.Options;
using OpenAI;

namespace Lister.Infrastructure.OpenAi;

public class CompletedJsonGetter(IOptions<OpenAiOptions> options) : IGetCompletedJson
{
    private readonly string _apiKey = options.Value.ApiKey;
    
    public async Task<string> Get(string example, string text, CancellationToken cancellationToken)
    {
        // var client = new OpenAIClient(_apiKey);
        // var chatCompletionsOptions = new ChatCompletionsOptions
        // {
        //     DeploymentName = "gpt-3.5-turbo",
        //     Messages =
        //     {
        //         new ChatRequestSystemMessage($"Here is an example of a JSON object: {example}"),
        //         new ChatRequestUserMessage("Can you convert the following text into that object's structure?"),
        //         new ChatRequestUserMessage(text)
        //     }
        // };
        //
        // client.Get
        // var response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
        // var retval = response.Value.Choices[0].Message.Content;
        // return retval;

        throw new NotImplementedException();
    }
}