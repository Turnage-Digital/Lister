namespace Lister.Lists.Domain.Entities;

public interface IWritableItem
{
    int? Id { get; set; }
    Guid? ListId { get; set; }
}