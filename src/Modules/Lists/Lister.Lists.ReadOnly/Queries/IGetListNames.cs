using Lister.Lists.ReadOnly.Dtos;

namespace Lister.Lists.ReadOnly.Queries;

public interface IGetListNames
{
    Task<ListNameDto[]> GetAsync(CancellationToken cancellationToken);
}