using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Queries;

public interface IGetListNames
{
    Task<ListName[]> GetAsync(CancellationToken cancellationToken);
}