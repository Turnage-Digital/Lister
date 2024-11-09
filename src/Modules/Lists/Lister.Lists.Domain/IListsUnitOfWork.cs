using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;

namespace Lister.Lists.Domain;

public interface IListsUnitOfWork<TList> : IUnitOfWork
    where TList : IWritableList
{
    IListsStore<TList> ListsStore { get; }
}