using AutoMapper;
using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.Endpoints.Lists.GetListById;

public class GetListByIdQueryHandler : GetListByIdQueryHandler<ListView>
{
    private readonly ListerDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListByIdQueryHandler(ListerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public override async Task<ListView> Handle(GetListByIdQuery<ListView> request, CancellationToken cancellationToken)
    {
        var parsed = Guid.Parse(request.Id);
        var pageNumber = request.PageNumber ?? 0;
        var pageSize = request.PageSize ?? 10;
        var entity = await _dbContext.Lists
            .Include(list => list.Statuses)
            .Include(list => list.Columns)
            .Include(list => list.Items
                .OrderBy(item => item.Id)
                .Skip(pageNumber * pageSize)
                .Take(pageSize))
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == parsed)
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        var retval = _mapper.Map<ListView>(entity);
        retval.Count = await _dbContext.Items
            .CountAsync(item => item.ListId == parsed, cancellationToken);
        return retval;
    }
}

public abstract class GetListByIdQueryHandler<TList> : IRequestHandler<GetListByIdQuery<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(GetListByIdQuery<TList> request, CancellationToken cancellationToken);
}