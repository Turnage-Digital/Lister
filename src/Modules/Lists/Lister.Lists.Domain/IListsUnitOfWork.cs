using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

public interface IListsUnitOfWork<TList, TItem> : IUnitOfWork
    where TList : IWritableList
    where TItem : IWritableItem
{
    IListsStore<TList> ListsStore { get; }

    IItemsStore<TItem> ItemsStore { get; }
}