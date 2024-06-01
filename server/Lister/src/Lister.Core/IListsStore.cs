using Lister.Core.ValueObjects;

namespace Lister.Core;

public interface IListsStore<TList>
    where TList : IWritableList
{
    Task<TList> InitAsync(string createdBy, string name, CancellationToken cancellationToken);

    Task CreateAsync(TList list, CancellationToken cancellationToken);

    Task<TList?> ReadAsync(string id, CancellationToken cancellationToken);
    
    Task DeleteAsync(string id, CancellationToken cancellationToken);

    Task<TList?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task SetNameAsync(TList list, string name, CancellationToken cancellationToken);

    Task<string> GetNameAsync(TList list, CancellationToken cancellationToken);

    Task SetColumnsAsync(TList list, IEnumerable<Column> columns, CancellationToken cancellationToken);

    Task<Column[]> GetColumnsAsync(TList list, CancellationToken cancellationToken);

    Task SetStatusesAsync(TList list, IEnumerable<Status> statuses, CancellationToken cancellationToken);

    Task<Status[]> GetStatusesAsync(TList list, CancellationToken cancellationToken);

    Task SetCreatedByAsync(TList list, string userId, CancellationToken cancellationToken);

    Task<string> GetCreatedByAsync(TList list, CancellationToken cancellationToken);

    Task<DateTime> GetCreatedOnAsync(TList list, CancellationToken cancellationToken);

    Task<Item> InitItemAsync(TList list, string createdBy, object bag, CancellationToken cancellationToken);
}