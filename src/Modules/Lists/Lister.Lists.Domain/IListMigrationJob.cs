namespace Lister.Lists.Domain;

public interface IListMigrationJob
{
    Guid Id { get; }
    Guid ListId { get; }
}