using System.Text.Json;
using Lister.Domain;
using Lister.Domain.Views;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Lister.Application.Queries;

public class GetListItemDefinitionQueryHandler(
    IGetListItemDefinition query,
    IDistributedCache cache,
    ILogger<GetListItemDefinitionQueryHandler> logger)
    : IRequestHandler<GetListItemDefinitionQuery, ListItemDefinition?>
{
    public async Task<ListItemDefinition?> Handle(
        GetListItemDefinitionQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserId is null)
            throw new ArgumentNullException(nameof(request), "UserId is null");

        ListItemDefinition? retval;

        var parsed = Guid.Parse(request.ListId);
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
            retval = await query.Get(request.UserId, parsed, cancellationToken);
            await CacheDatabaseResultAsync(cacheKey, retval, cancellationToken);
        }

        return retval;
    }

    private async Task CacheDatabaseResultAsync(
        string key,
        ListItemDefinition? value,
        CancellationToken cancellationToken)
    {
        if (value == null)
            return;

        var serialized = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
        await cache.SetStringAsync(key, serialized, options, cancellationToken);
    }
}