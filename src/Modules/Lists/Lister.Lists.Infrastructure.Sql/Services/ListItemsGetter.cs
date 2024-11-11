using System.Text.Json;
using Dapper;
using Lister.Core.Domain;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class ListItemsGetter(ListsDbContext dbContext) : IGetListItems
{
    public async Task<PagedResponse<ListItem>> GetAsync(Guid listId, int page, int pageSize, string? field,
        string? sort, CancellationToken cancellationToken)
    {
        var builder = new SqlBuilder();
        const string sql = """
                           SELECT SQL_CALC_FOUND_ROWS
                               i.Bag, i.CreatedBy, i.CreatedOn, i.Id
                           FROM
                               Items i
                           WHERE
                               i.ListId = @listId
                           /**orderby**/
                           LIMIT @pageSize OFFSET @offset; SELECT FOUND_ROWS();
                           """;

        var parameters = new
        {
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
        var items = data.Select(d => new ListItem
        {
            Bag = JsonSerializer.Deserialize<object>(d.Bag),
            Id = d.Id,
            ListId = listId
        }).ToList();
        var count = await multi.ReadSingleAsync<long>();

        var retval = new PagedResponse<ListItem>(items, count);
        return retval;
    }
}