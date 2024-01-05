using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListNames;

public class GetListNamesQueryHandler : GetListNamesQueryHandler<ListNameView>
{
    private readonly ListerDbContext _dbContext;

    public GetListNamesQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<ListNameView[]> Handle(
        GetListNamesQuery<ListNameView> request,
        CancellationToken cancellationToken
    )
    {
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Select(list => new ListNameView
            {
                Id = list.Id,
                Name = list.Name
            })
            .ToArrayAsync(cancellationToken);
        return retval;
    }
}

public abstract class GetListNamesQueryHandler<TList>
    : IRequestHandler<GetListNamesQuery<TList>, TList[]>
    where TList : IReadOnlyList
{
    public abstract Task<TList[]> Handle(
        GetListNamesQuery<TList> request,
        CancellationToken cancellationToken
    );
}