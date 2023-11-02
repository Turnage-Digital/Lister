using Lister.Core.ValueObjects;

namespace Lister.Core;

public interface IListDefsStore<TListDef>
    where TListDef : IWritableListDef
{
    Task<TListDef> InitAsync(CancellationToken cancellationToken);

    Task CreateAsync(TListDef listDef, CancellationToken cancellationToken);

    Task<TListDef?> ReadAsync(string id, CancellationToken cancellationToken);

    Task SetCreatedByAsync(TListDef listDef, string userId, CancellationToken cancellationToken);

    Task<string> GetCreatedByAsync(TListDef listDef, CancellationToken cancellationToken);

    Task<DateTime> GetCreatedOnAsync(TListDef listDef, CancellationToken cancellationToken);

    Task SetNameAsync(TListDef listDef, string name, CancellationToken cancellationToken);

    Task<string> GetNameAsync(TListDef listDef, CancellationToken cancellationToken);

    Task SetStatusDefsAsync(TListDef listDef, StatusDef[] statusDefs, CancellationToken cancellationToken);

    Task<StatusDef[]> GetStatusDefsAsync(TListDef listDef, CancellationToken cancellationToken);

    Task SetColumnDefsAsync(TListDef listDef, ColumnDef[] columnDefs, CancellationToken cancellationToken);

    Task<ColumnDef[]> GetColumnDefsAsync(TListDef listDef, CancellationToken cancellationToken);
}