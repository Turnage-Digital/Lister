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
        var entity = await _dbContext.Lists
            .Include(list => list.Statuses)
            .Include(list => list.Columns)
            .Include(list => list.Items)
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == request.Id)
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        var retval = _mapper.Map<ListView>(entity);
        return retval;
    }
}

public abstract class GetListByIdQueryHandler<TList> : IRequestHandler<GetListByIdQuery<TList>, TList>
    where TList : IReadOnlyList
{
    public abstract Task<TList> Handle(GetListByIdQuery<TList> request, CancellationToken cancellationToken);
}