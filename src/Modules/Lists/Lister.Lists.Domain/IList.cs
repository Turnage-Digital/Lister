namespace Lister.Lists.Domain;

public interface IList
{
    Guid? Id { get; set; }
    string Name { get; }
}