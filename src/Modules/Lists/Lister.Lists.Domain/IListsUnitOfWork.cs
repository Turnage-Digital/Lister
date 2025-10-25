using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

public interface IListsUnitOfWork<TList, TItem, TMigrationJob> : IUnitOfWork
    where TList : IWritableList
    where TItem : IWritableItem
    where TMigrationJob : IWritableListMigrationJob
{
    IListsStore<TList> ListsStore { get; }

    IItemsStore<TItem> ItemsStore { get; }

    IListMigrationJobStore<TMigrationJob> MigrationJobsStore { get; }
}