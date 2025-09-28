using Lister.Lists.Application.Endpoints.Migrations;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests.Migrations;

[TestFixture]
public class MigrationExecutorTests
{
    [Test]
    public async Task Execute_TightenConstraints_UpdatesColumns_And_PublishesEvents()
    {
        var listId = Guid.NewGuid();
        const string user = "tester";

        var uow = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        var listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();
        uow.SetupGet(x => x.ListsStore).Returns(listsStore.Object);
        uow.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        var list = new ListDb { Id = listId, Name = "Sample" };
        listsStore.Setup(x => x.GetByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(list);
        listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = false }
            ]);
        listsStore.Setup(x => x.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        itemsStore.Setup(x => x.GetItemIdsAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var mediator = new Mock<IMediator>();

        var executor = new MigrationExecutor<ListDb, ItemDb>(uow.Object, mediator.Object);
        var plan = new MigrationPlan
        {
            TightenConstraints =
            [
                new TightenConstraintsOp("prop1", true, null, null, null, null)
            ]
        };

        var result = await executor.ExecuteAsync(listId, user, plan, CancellationToken.None);

        Assert.That(result.IsSafe, Is.True);
        listsStore.Verify(x => x.SetColumnsAsync(list,
            It.Is<IEnumerable<Column>>(cols => cols.Any(c => c.StorageKey == "prop1" && c.Required)),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        mediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}