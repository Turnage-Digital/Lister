namespace Lister.Core.Domain.Services;

public interface IGetCompletedJson
{
    Task<string> Get(string example, string text, CancellationToken cancellationToken);
}