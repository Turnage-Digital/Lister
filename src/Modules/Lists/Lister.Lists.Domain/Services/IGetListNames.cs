using Lister.Lists.Domain.Views;

namespace Lister.Lists.Domain.Services;

public interface IGetListNames
{
    Task<ListName[]> Get(string userId, CancellationToken cancellationToken);
}