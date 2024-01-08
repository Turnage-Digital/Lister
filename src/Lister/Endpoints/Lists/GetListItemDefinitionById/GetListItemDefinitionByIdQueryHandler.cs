using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListItemDefinitionById;

public class GetListItemDefinitionByIdQueryHandler
    : GetListItemDefinitionByIdQueryHandler<ListItemDefinitionView>
{
    private readonly ListerDbContext _dbContext;

    public GetListItemDefinitionByIdQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<ListItemDefinitionView> Handle(
        GetListItemDefinitionByIdQuery<ListItemDefinitionView> request,
        CancellationToken cancellationToken
    )
    {
        var parsed = Guid.Parse(request.Id);
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == parsed)
            .Select(list => new ListItemDefinitionView
            {
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
            }).SingleAsync(cancellationToken);
        return retval;
    }
}

public abstract class GetListItemDefinitionByIdQueryHandler<TList>
    : IRequestHandler<GetListItemDefinitionByIdQuery<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(
        GetListItemDefinitionByIdQuery<TList> request,
        CancellationToken cancellationToken
    );
}