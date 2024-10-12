using Lister.Core.Entities;

namespace Lister.Core;

public interface IListerUnitOfWork<TList, TItem> : IUnitOfWork
    where TList : IWritableList
    where TItem : Item
{
    IListsStore<TList, TItem> ListsStore { get; }

    IItemsStore<TItem> ItemsStore { get; }
}