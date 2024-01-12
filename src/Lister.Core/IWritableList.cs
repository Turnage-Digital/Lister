namespace Lister.Core;

public interface IWritableList : IList
{
    public string GetId() => Id?.ToString() ?? throw new InvalidOperationException();
}