using System.Text.Json;
using Lister.Lists.ReadOnly.Dtos;
using Lister.Lists.ReadOnly.Queries;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Lister.Lists.Application.Endpoints.Queries.GetListItemDefinition;

public class GetListItemDefinitionQueryHandler(
    IGetListItemDefinition query,
    IDistributedCache cache,
    ILogger<GetListItemDefinitionQueryHandler> logger
)
    : IRequestHandler<GetListItemDefinitionQuery, ListItemDefinitionDto?>
{
    public async Task<ListItemDefinitionDto?> Handle(
        GetListItemDefinitionQuery request,
        CancellationToken cancellationToken
    )
    {
        ListItemDefinitionDto? retval;

        var cacheKey = $"ListItemDefinition-{request.ListId}";
        var cacheValue = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cacheValue != null)
        {
            logger.LogInformation("Cache hit for {cacheKey}", cacheKey);
            retval = JsonSerializer.Deserialize<ListItemDefinitionDto>(cacheValue);
        }
        else
        {
            logger.LogInformation("Cache miss for {cacheKey}", cacheKey);
            retval = await query.GetAsync(request.ListId, cancellationToken);
            await CacheDatabaseResultAsync(cacheKey, retval, cancellationToken);
        }

        return retval;
    }

    private async Task CacheDatabaseResultAsync(
        string key,
        ListItemDefinitionDto? value,
        CancellationToken cancellationToken
    )
    {
        if (value is null)
        {
            return;
        }

        var serialized = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
        await cache.SetStringAsync(key, serialized, options, cancellationToken);
    }
}