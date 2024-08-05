using System.Text.Json;
using Dapper;
using Lister.Application.Queries;
using Lister.Core.SqlDB;
using Lister.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Application.SqlDB.Queries;

public class GetListItemsQueryHandler(ListerDbContext dbContext)
    : GetListItemsQueryHandlerBase
{
    public override async Task<PagedResponse<Item>> Handle(
        GetListItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var builder = new SqlBuilder();
        var sql =
            """
            SELECT SQL_CALC_FOUND_ROWS
                i.Bag, i.CreatedBy, i.CreatedOn, i.Id
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

        var connection = dbContext.Database.GetDbConnection();
        var multi = await connection.QueryMultipleAsync(template.RawSql, template.Parameters);

        var data = await multi.ReadAsync<dynamic>();
        var items = data.Select(d => new Item
        {
            Bag = JsonSerializer.Deserialize<object>(d.Bag),
            CreatedBy = d.CreatedBy,
            CreatedOn = d.CreatedOn,
            Id = d.Id
        }).ToList();
        var count = await multi.ReadSingleAsync<long>();

        var retval = new PagedResponse<Item>(items, count);
        return retval;
    }
}