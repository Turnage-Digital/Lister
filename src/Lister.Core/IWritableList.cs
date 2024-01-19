namespace Lister.Core;

public interface IWritableList : IList
{
    public string GetId()
    {
        return Id?.ToString() ?? throw new InvalidOperationException();
    }
}