using System.Text.Json;
using Dapper;
using Lister.Lists.ReadOnly.Queries;
using Lister.Lists.ReadOnly.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Infrastructure.Sql.Services;

public class PagedListGetter(ListsDbContext dbContext) : IGetPagedList
{
    public async Task<PagedListDto> GetAsync(
        Guid listId,
        int page,
        int pageSize,
        string? field,
        string? sort,
        CancellationToken cancellationToken
    )
    {
        var listName = await dbContext.Lists
            .Where(l => l.Id == listId)
            .Select(l => l.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var builder = new SqlBuilder();
        const string sql = """
                           SELECT SQL_CALC_FOUND_ROWS
                               i.Bag, i.Id, i.ListId
                           FROM
                               Items i
                           WHERE
                               i.ListId = @listId AND i.IsDeleted = 0
                           /**orderby**/
                           LIMIT @pageSize OFFSET @offset; 
                           SELECT FOUND_ROWS();
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

        var read = await multi.ReadAsync<dynamic>();
        var items = read.Select(d => new ListItemDto
            {
                Bag = JsonSerializer.Deserialize<object>(d.Bag),
                Id = d.Id,
                ListId = listId
            })
            .ToArray();
        var count = await multi.ReadSingleAsync<long>();

        var retval = new PagedListDto
        {
            Id = listId,
            Count = count,
            Items = items,
            Name = listName
        };
        return retval;
    }
}
