namespace Lister.Lists.Domain.Entities;

public interface IItem
{
    int? Id { get; set; }

    Guid? ListId { get; set; }
}