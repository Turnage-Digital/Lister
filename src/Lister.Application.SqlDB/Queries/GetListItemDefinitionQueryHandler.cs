using Lister.Application.Queries;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Lister.Application.SqlDB.Queries;

public class GetListItemDefinitionQueryHandler(
    ListerDbContext dbContext,
    IDistributedCache cache,
    ILogger<GetListItemDefinitionQueryHandler> logger)
    : GetListItemDefinitionQueryHandlerBase<ListItemDefinitionView?>
{
    public override async Task<ListItemDefinitionView?> Handle(
        GetListItemDefinitionQuery<ListItemDefinitionView?> request,
        CancellationToken cancellationToken
    )
    {
        ListItemDefinitionView? retval;

        var parsed = Guid.Parse(request.ListId);
        var cacheKey = $"ListItemDefinition-{request.ListId}";
        var cacheValue = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cacheValue != null)
        {
            logger.LogInformation("Cache hit for {cacheKey}", cacheKey);
            retval = JsonConvert.DeserializeObject<ListItemDefinitionView>(cacheValue);
        }
        else
        {
            logger.LogInformation("Cache miss for {cacheKey}", cacheKey);
            retval = await GetFromDatabaseAsync(parsed, request.UserId, cancellationToken);
            await CacheDatabaseResultAsync(cacheKey, retval, cancellationToken);
        }

        return retval;
    }

    private async Task<ListItemDefinitionView?> GetFromDatabaseAsync(
        Guid listId,
        string? userId,
        CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Where(list => list.CreatedBy == userId)
            .Where(list => list.Id == listId)
            .Select(list => new ListItemDefinitionView
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
        ListItemDefinitionView? value,
        CancellationToken cancellationToken)
    {
        if (value == null)
            return;

        var serialized = JsonConvert.SerializeObject(value);
        var options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
        await cache.SetStringAsync(key, serialized, options, cancellationToken);
    }
}