using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.Services;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lister.Lists.Tests.Infrastructure;

[TestFixture]
public class ListItemDefinitionGetterTests
{
    [Test]
    public async Task GetAsync_ReturnsColumns_WithStorageKeys()
    {
        var options = new DbContextOptionsBuilder<ListsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ListsDbContext(options);

        var listId = Guid.NewGuid();
        var list = new ListDb
        {
            Id = listId,
            Name = "Sample"
        };

        list.Columns.Add(new ColumnDb
        {
            StorageKey = "prop1",
            Name = "Title",
            Type = ColumnType.Text,
            Required = true,
            ListDb = list
        });

        context.Lists.Add(list);
        await context.SaveChangesAsync();

        var getter = new ListItemDefinitionGetter(context);
        var definition = await getter.GetAsync(listId, CancellationToken.None);

        Assert.That(definition, Is.Not.Null);
        Assert.That(definition!.Columns.Any(c => c.StorageKey == "prop1"), Is.True);
    }
}
