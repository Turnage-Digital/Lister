namespace Lister.Services.OpenAi;

public interface IOpenAIService
{
    Task<string> CreateCompletion(string model, string[] messages);
}