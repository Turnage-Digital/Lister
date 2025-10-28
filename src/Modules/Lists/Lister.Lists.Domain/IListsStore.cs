using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.ValueObjects;

namespace Lister.Lists.Domain;

public interface IListsStore<TList>
    where TList : IWritableList
{
    Task<TList> InitAsync(string name, string createdBy, CancellationToken cancellationToken);
    Task<TList?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TList?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task CreateAsync(TList list, CancellationToken cancellationToken);
    Task DeleteAsync(TList list, string deletedBy, CancellationToken cancellationToken);
    Task RenameAsync(TList list, string newName, string actedBy, CancellationToken cancellationToken);

    Task SetColumnsAsync(
        TList list,
        IEnumerable<Column> columns,
        string actedBy,
        CancellationToken cancellationToken
    );

    Task<Column[]> GetColumnsAsync(TList list, CancellationToken cancellationToken);

    Task SetStatusesAsync(
        TList list,
        IEnumerable<Status> statuses,
        string actedBy,
        CancellationToken cancellationToken
    );

    Task<Status[]> GetStatusesAsync(TList list, CancellationToken cancellationToken);

    Task SetStatusTransitionsAsync(
        TList list,
        IEnumerable<StatusTransition> transitions,
        string actedBy,
        CancellationToken cancellationToken
    );

    Task<StatusTransition[]> GetStatusTransitionsAsync(TList list, CancellationToken cancellationToken);
}
