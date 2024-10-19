namespace Lister.Domain;

public interface IListerUnitOfWork<TList> : IUnitOfWork
    where TList : IWritableList
{
    IListsStore<TList> ListsStore { get; }
}