using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain;

public interface IListsStore<TList>
    where TList : IWritableList
{
    Task<TList?> GetByIdAsync(string userId, Guid id, CancellationToken cancellationToken);

    Task<TList?> GetByNameAsync(string userId, string name, CancellationToken cancellationToken);

    Task<TList> InitAsync(string createdBy, string name, CancellationToken cancellationToken);

    Task CreateAsync(TList list, CancellationToken cancellationToken);

    Task DeleteAsync(TList list, string deletedBy, CancellationToken cancellationToken);

    Task SetColumnsAsync(TList list, IEnumerable<Column> columns, CancellationToken cancellationToken);

    Task<Column[]> GetColumnsAsync(TList list, CancellationToken cancellationToken);

    Task SetStatusesAsync(TList list, IEnumerable<Status> statuses, CancellationToken cancellationToken);

    Task<Status[]> GetStatusesAsync(TList list, CancellationToken cancellationToken);

    Task<Item> AddItemAsync(TList list, string createdBy, object bag, CancellationToken cancellationToken);
}