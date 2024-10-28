using System.Text.Json;
using Dapper;
using Lister.Core.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemsGetter(ListerDbContext dbContext) : IGetListItems
{
    public async Task<PagedResponse<Item>> Get(string userId,
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken)
    {
        var builder = new SqlBuilder();
        const string sql = """
                           SELECT SQL_CALC_FOUND_ROWS
                               i.Bag, i.CreatedBy, i.CreatedOn, i.Id
                           FROM
                               Items i
                           WHERE
                               i.ListId = @listId AND i.CreatedBy = @userId
                           /**orderby**/
                           LIMIT @pageSize OFFSET @offset; SELECT FOUND_ROWS();
                           """;

        var parameters = new
        {
            userId,
            listId,
            pageSize,
            offset = page * pageSize
        };

        var template = builder.AddTemplate(sql, parameters);

        var orderBy = field is not null
            ? $"JSON_EXTRACT(i.Bag, '$.{field}') {sort}"
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