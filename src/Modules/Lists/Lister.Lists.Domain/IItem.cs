namespace Lister.Lists.Domain;

public interface IItem
{
    int? Id { get; set; }
    Guid? ListId { get; set; }
}