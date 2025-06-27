using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql;

public class ListsStore(ListsDbContext dbContext)
    : IListsStore<ListDb>
{
    private readonly EntityStore<ListDb> _entityStore = new(dbContext);

    public async Task<ListDb?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.Id == id)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public async Task<ListDb?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var retval = await dbContext.Lists
            .Include(l => l.Columns)
            .Include(l => l.Statuses)
            .Where(list => list.Name == name)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);
        return retval;
    }

    public Task<ListDb> InitAsync(string name, string createdBy, CancellationToken cancellationToken)
    {
        var retval = new ListDb
        {
            Name = name
        };
        retval.History.Add(new ListHistoryEntryDb
        {
            Type = ListHistoryType.Created,
            On = DateTime.UtcNow,
            By = createdBy,
            List = retval
        });
        return Task.FromResult(retval);
    }

    public async Task CreateAsync(ListDb listDb, CancellationToken cancellationToken)
    {
        await _entityStore.CreateAsync(listDb, cancellationToken);
    }

    public Task DeleteAsync(ListDb listDb, string deletedBy, CancellationToken cancellationToken)
    {
        listDb.IsDeleted = true;
        listDb.History.Add(new ListHistoryEntryDb
        {
            Type = ListHistoryType.Deleted,
            On = DateTime.UtcNow,
            By = deletedBy,
            List = listDb
        });
        return Task.CompletedTask;
    }

    public Task SetColumnsAsync(
        ListDb listDb,
        IEnumerable<Column> columns,
        CancellationToken cancellationToken
    )
    {
        listDb.Columns = columns
            .Select(pd => new ColumnDb { Name = pd.Name, Type = pd.Type, ListDb = listDb })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Column[]> GetColumnsAsync(ListDb listDb, CancellationToken cancellationToken)
    {
        var retval = listDb.Columns
            .Select(pd => new Column { Name = pd.Name, Type = pd.Type })
            .ToArray();
        return Task.FromResult(retval);
    }

    public Task SetStatusesAsync(
        ListDb listDb,
        IEnumerable<Status> statuses,
        CancellationToken cancellationToken
    )
    {
        listDb.Statuses = statuses
            .Select(sd => new StatusDb { Name = sd.Name, Color = sd.Color, ListDb = listDb })
            .ToList();
        return Task.CompletedTask;
    }

    public Task<Status[]> GetStatusesAsync(ListDb listDb, CancellationToken cancellationToken)
    {
        var retval = listDb.Statuses
            .Select(sd => new Status { Name = sd.Name, Color = sd.Color })
            .ToArray();
        return Task.FromResult(retval);
    }
}