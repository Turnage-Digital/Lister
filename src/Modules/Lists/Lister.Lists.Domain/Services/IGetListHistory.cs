using Lister.Core.Domain.ValueObjects;
using Lister.Lists.Domain.Enums;

namespace Lister.Lists.Domain.Services;

public interface IGetListHistory
{
    Task<HistoryPage<ListHistoryType>> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
}