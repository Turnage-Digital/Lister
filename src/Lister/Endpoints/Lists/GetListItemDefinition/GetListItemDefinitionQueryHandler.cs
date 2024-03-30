using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListItemDefinition;

public class GetListItemDefinitionQueryHandler
    : GetListItemDefinitionByIdQueryHandler<ListItemDefinitionView>
{
    private readonly ListerDbContext _dbContext;

    public GetListItemDefinitionQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<ListItemDefinitionView> Handle(
        GetListItemDefinitionQuery<ListItemDefinitionView> request,
        CancellationToken cancellationToken
    )
    {
        var parsed = Guid.Parse(request.ListId);
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == parsed)
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
            .SingleAsync(cancellationToken);
        return retval;
    }
}

public abstract class GetListItemDefinitionByIdQueryHandler<TList>
    : IRequestHandler<GetListItemDefinitionQuery<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(
        GetListItemDefinitionQuery<TList> request,
        CancellationToken cancellationToken
    );
}