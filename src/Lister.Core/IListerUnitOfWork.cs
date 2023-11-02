namespace Lister.Core;

public interface IListerUnitOfWork<TListDef> : IUnitOfWork
    where TListDef : IWritableListDef
{
    IListDefsStore<TListDef> ListDefsStore { get; }
}