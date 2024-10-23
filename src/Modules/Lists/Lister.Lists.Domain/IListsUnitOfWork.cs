using Lister.Core.Domain;

namespace Lister.Lists.Domain;

public interface IListsUnitOfWork<TList> : IUnitOfWork
    where TList : IWritableList
{
    IListsStore<TList> ListsStore { get; }
}