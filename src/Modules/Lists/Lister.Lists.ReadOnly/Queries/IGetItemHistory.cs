using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetItemHistory
{
    Task<HistoryPage<ItemHistoryType>> GetAsync(
        Guid listId,
        int itemId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
}
