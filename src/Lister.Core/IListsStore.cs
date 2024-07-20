using Lister.Core.ValueObjects;

namespace Lister.Core;

public interface IListsStore<TList, in TItem>
    where TList : IWritableList
{
    Task<TList> InitAsync(string createdBy, string name, CancellationToken cancellationToken);

    Task CreateAsync(TList list, CancellationToken cancellationToken);

    Task<TList?> ReadAsync(string id, CancellationToken cancellationToken);

    Task DeleteAsync(TList list, string deletedBy, CancellationToken cancellationToken);

    Task<TList?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task SetColumnsAsync(TList list, IEnumerable<Column> columns, CancellationToken cancellationToken);

    Task<Column[]> GetColumnsAsync(TList list, CancellationToken cancellationToken);

    Task SetStatusesAsync(TList list, IEnumerable<Status> statuses, CancellationToken cancellationToken);

    Task<Status[]> GetStatusesAsync(TList list, CancellationToken cancellationToken);

    Task AddItemAsync(TList list, TItem item, CancellationToken cancellationToken);
}