namespace Lister.Services.OpenAI;

public interface IOpenAIService
{
    Task<string> CreateCompletion(string model, string[] messages);
}