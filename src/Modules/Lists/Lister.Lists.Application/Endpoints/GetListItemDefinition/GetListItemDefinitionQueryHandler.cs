using System.Text.Json;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Lister.Lists.Application.Endpoints.GetListItemDefinition;

public class GetListItemDefinitionQueryHandler(
    IGetListItemDefinition query,
    IDistributedCache cache,
    ILogger<GetListItemDefinitionQueryHandler> logger
)
    : IRequestHandler<GetListItemDefinitionQuery, ListItemDefinition?>
{
    public async Task<ListItemDefinition?> Handle(
        GetListItemDefinitionQuery request,
        CancellationToken cancellationToken
    )
    {
        ListItemDefinition? retval;

        var cacheKey = $"ListItemDefinition-{request.ListId}";
        var cacheValue = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cacheValue != null)
        {
            logger.LogInformation("Cache hit for {cacheKey}", cacheKey);
            retval = JsonSerializer.Deserialize<ListItemDefinition>(cacheValue);
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
        ListItemDefinition? value,
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