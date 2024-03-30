using Dapper;
using Lister.Core.SqlDB;
using Lister.Core.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.GetListItems;

public class GetListItemsQueryHandler : IRequestHandler<GetListItemsQuery, GetListItemsResponse>
{
    private readonly ListerDbContext _dbContext;

    public GetListItemsQueryHandler(ListerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetListItemsResponse> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var builder = new SqlBuilder();
        var sql =
            """
            SELECT SQL_CALC_FOUND_ROWS
                i.Id, i.Bag
            FROM
                Items i
            WHERE
                i.ListId = @listId
            /**orderby**/
            """;

        sql += request.PageSize is not null
            ? " LIMIT @pageSize OFFSET @offset; SELECT FOUND_ROWS();"
            : "; SELECT FOUND_ROWS();";

        var parameters = new
        {
            listId = request.ListId,
            pageSize = request.PageSize,
            offset = request.Page * request.PageSize
        };

        var template = builder.AddTemplate(sql, parameters);

        var orderBy = request.Field is not null
            ? $"JSON_EXTRACT(i.Bag, '$.{request.Field}') {request.Sort}"
            : "i.Id asc";
        builder.OrderBy(orderBy);

        var connection = _dbContext.Database.GetDbConnection();
        var multi = await connection.QueryMultipleAsync(template.RawSql, template.Parameters);

        var data = await multi.ReadAsync<dynamic>();
        var items = data.Select(d => new Item
        {
            Id = d.Id,
            Bag = JsonConvert.DeserializeObject(d.Bag)
        }).ToList();

        var count = await multi.ReadSingleAsync<long>();
        var retval = new GetListItemsResponse(items, count);
        return retval;
    }
}