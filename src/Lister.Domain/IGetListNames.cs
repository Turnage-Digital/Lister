using Lister.Domain.Views;

namespace Lister.Domain;

public interface IGetListNames
{
    Task<ListName[]> Get(string userId, CancellationToken cancellationToken);
}