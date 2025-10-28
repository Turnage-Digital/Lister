namespace Lister.Lists.Domain.Entities;

public interface IWritableList
{
    Guid? Id { get; set; }
    string Name { get; set; }
}
