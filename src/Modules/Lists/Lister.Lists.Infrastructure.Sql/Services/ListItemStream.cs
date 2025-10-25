using System.Runtime.CompilerServices;
using Lister.Lists.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemStream(ListsDbContext dbContext) : IGetListItemStream
{
    public async IAsyncEnumerable<int> StreamAsync(
        Guid listId,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        await foreach (var id in dbContext.Items
                           .Where(i => i.ListId == listId && i.Id != null)
                           .OrderBy(i => i.Id)
                           .Select(i => i.Id!.Value)
                           .AsAsyncEnumerable()
                           .WithCancellation(cancellationToken))
        {
            yield return id;
        }
    }
}