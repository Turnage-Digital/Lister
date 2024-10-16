using System.Text.Json;
using Lister.Application.Queries;
using Lister.Core.Entities.Views;
using Lister.Core.Sql;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Lister.Application.Sql.Queries;

public class GetListItemDefinitionQueryHandler(
    ListerDbContext dbContext,
    IDistributedCache cache,
    ILogger<GetListItemDefinitionQueryHandler> logger)
    : GetListItemDefinitionQueryHandlerBase<ListItemDefinition?>
{
    public override async Task<ListItemDefinition?> Handle(
        GetListItemDefinitionQuery<ListItemDefinition?> request,
        CancellationToken cancellationToken
    )
    {
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
            retval = await GetFromDatabaseAsync(parsed, request.UserId, cancellationToken);
            await CacheDatabaseResultAsync(cacheKey, retval, cancellationToken);
        }

        return retval;
    }

    private async Task<ListItemDefinition?> GetFromDatabaseAsync(
        Guid listId,
        string? userId,
        CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == listId)
            .Select(list => new ListItemDefinition
            {
                Id = list.Id,
                Name = list.Name,
                Columns = list.Columns
                    .Select(column => new Column
                    {
                        Name = column.Name,
                        Type = column.Type
                    }).ToArray(),
                Statuses = list.Statuses
                    .Select(status => new Status
                    {
                        Name = status.Name,
                        Color = status.Color
                    }).ToArray()
            })
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
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