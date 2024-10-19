namespace Lister.Domain;

public interface IGetCompletedJson
{
    Task<string> Get(string example, string text, CancellationToken cancellationToken);
}