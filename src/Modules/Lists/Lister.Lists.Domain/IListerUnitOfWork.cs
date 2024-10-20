using Lister.Core.Domain;

namespace Lister.Lists.Domain;

public interface IListerUnitOfWork<TList> : IUnitOfWork
    where TList : IWritableList
{
    IListsStore<TList> ListsStore { get; }
}