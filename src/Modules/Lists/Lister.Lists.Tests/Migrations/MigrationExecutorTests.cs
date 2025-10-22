using Lister.Core.Domain.IntegrationEvents;
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
    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _uow = null!;
    private Mock<IListsStore<ListDb>> _listsStore = null!;
    private Mock<IItemsStore<ItemDb>> _itemsStore = null!;
    private Mock<IMediator> _mediator = null!;
    private ListDb _list = null!;
    private Guid _listId;
    private const string User = "tester";

    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _listsStore = new Mock<IListsStore<ListDb>>();
        _itemsStore = new Mock<IItemsStore<ItemDb>>();
        _mediator = new Mock<IMediator>();

        _uow.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _uow.SetupGet(x => x.ItemsStore).Returns(_itemsStore.Object);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _mediator.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _listId = Guid.NewGuid();
        _list = new ListDb { Id = _listId, Name = "Sample" };

        _listsStore.Setup(x => x.GetByIdAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_list);
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Column>());
        _listsStore.Setup(x => x.GetStatusesAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Status>());
        _listsStore.Setup(x => x.GetStatusTransitionsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<StatusTransition>());
        _listsStore.Setup(x => x.SetColumnsAsync(
                _list,
                It.IsAny<IEnumerable<Column>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _listsStore.Setup(x => x.SetStatusesAsync(
                _list,
                It.IsAny<IEnumerable<Status>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _itemsStore.Setup(x => x.GetItemIdsAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<int>());
        _itemsStore.Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private MigrationExecutor<ListDb, ItemDb> CreateExecutor() =>
        new(_uow.Object, _mediator.Object);

    [Test]
    public async Task ExecuteAsync_TightenConstraints_UpdatesMetadata_AndPublishesEvents()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = false }
            ]);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            TightenConstraints = [new TightenConstraintsOp("prop1", true, null, null, null, null)]
        };

        var result = await executor.ExecuteAsync(_listId, User, plan, CancellationToken.None);

        Assert.That(result.IsSafe, Is.True);
        _listsStore.Verify(x => x.SetColumnsAsync(
            _list,
            It.Is<IEnumerable<Column>>(cols => cols.Any(c => c.StorageKey == "prop1" && c.Required)),
            User,
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _mediator.Verify(m => m.Publish(
            It.Is<ListMigrationStartedIntegrationEvent>(evt => evt.ListId == _listId),
            It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(m => m.Publish(
            It.Is<ListMigrationCompletedIntegrationEvent>(evt => evt.ListId == _listId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_RenameStorageKeys_UpdatesColumns()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true }
            ]);

        Column[]? persisted = null;
        _listsStore.Setup(x => x.SetColumnsAsync(
                _list,
                It.IsAny<IEnumerable<Column>>(),
                User,
                It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Column>, string, CancellationToken>((_, cols, _, _) =>
            {
                persisted = cols.ToArray();
            })
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            RenameStorageKeys = [new RenameStorageKeyOp("prop1", "propMain")]
        };

        await executor.ExecuteAsync(_listId, User, plan, CancellationToken.None);

        Assert.That(persisted, Is.Not.Null);
        Assert.That(persisted!.Single().StorageKey, Is.EqualTo("propMain"));
    }

    [Test]
    public async Task ExecuteAsync_ChangeColumnTypes_ConvertsBagValues()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Total", Type = ColumnType.Text, Required = false }
            ]);
        _itemsStore.Setup(x => x.GetItemIdsAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([1]);
        var item = new ItemDb { Id = 1, ListId = _listId };
        _itemsStore.Setup(x => x.GetByIdAsync(1, _listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, object?> { ["prop1"] = "42" });

        Dictionary<string, object?>? convertedBag = null;
        _itemsStore.Setup(x => x.SetBagAsync(item, It.IsAny<object>(), User, It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((_, bag, _, _) =>
            {
                convertedBag = bag as Dictionary<string, object?>;
            })
            .Returns(Task.CompletedTask);

        Column[]? persisted = null;
        _listsStore.Setup(x => x.SetColumnsAsync(
                _list,
                It.IsAny<IEnumerable<Column>>(),
                User,
                It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Column>, string, CancellationToken>((_, cols, _, _) =>
            {
                persisted = cols.ToArray();
            })
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            ChangeColumnTypes = [new ChangeColumnTypeOp("prop1", ColumnType.Number, "parseNumber")]
        };

        await executor.ExecuteAsync(_listId, User, plan, CancellationToken.None);

        Assert.That(convertedBag, Is.Not.Null);
        Assert.That(convertedBag!["prop1"], Is.TypeOf<decimal>());
        Assert.That(persisted, Is.Not.Null);
        Assert.That(persisted!.Single().Type, Is.EqualTo(ColumnType.Number));
    }

    [Test]
    public async Task ExecuteAsync_RemoveColumns_DeletesMetadataAndBagEntries()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true },
                new Column { StorageKey = "prop2", Name = "Obsolete", Type = ColumnType.Text, Required = false }
            ]);

        _itemsStore.Setup(x => x.GetItemIdsAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([1]);
        var item = new ItemDb { Id = 1, ListId = _listId };
        _itemsStore.Setup(x => x.GetByIdAsync(1, _listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, object?> { ["prop1"] = "keep", ["prop2"] = "remove" });

        Dictionary<string, object?>? updatedBag = null;
        _itemsStore.Setup(x => x.SetBagAsync(item, It.IsAny<object>(), User, It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((_, bag, _, _) =>
            {
                updatedBag = bag as Dictionary<string, object?>;
            })
            .Returns(Task.CompletedTask);

        Column[]? persisted = null;
        _listsStore.Setup(x => x.SetColumnsAsync(
                _list,
                It.IsAny<IEnumerable<Column>>(),
                User,
                It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Column>, string, CancellationToken>((_, cols, _, _) =>
            {
                persisted = cols.ToArray();
            })
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            RemoveColumns = [new RemoveColumnOp("prop2", "drop")]
        };

        await executor.ExecuteAsync(_listId, User, plan, CancellationToken.None);

        Assert.That(updatedBag, Is.Not.Null);
        Assert.That(updatedBag!.ContainsKey("prop2"), Is.False);
        Assert.That(persisted, Is.Not.Null);
        Assert.That(persisted!.Any(c => c.StorageKey == "prop2"), Is.False);
    }

    [Test]
    public async Task ExecuteAsync_RemoveStatuses_MapsExistingItems()
    {
        _listsStore.Setup(x => x.GetStatusesAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Status { Name = "Active", Color = "green" },
                new Status { Name = "Closed", Color = "gray" }
            ]);
        _itemsStore.Setup(x => x.GetItemIdsAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([1]);
        var item = new ItemDb { Id = 1, ListId = _listId };
        _itemsStore.Setup(x => x.GetByIdAsync(1, _listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, object?> { ["status"] = "Active" });

        Dictionary<string, object?>? mappedBag = null;
        _itemsStore.Setup(x => x.SetBagAsync(item, It.IsAny<object>(), User, It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((_, bag, _, _) =>
            {
                mappedBag = bag as Dictionary<string, object?>;
            })
            .Returns(Task.CompletedTask);

        Status[]? persistedStatuses = null;
        _listsStore.Setup(x => x.SetStatusesAsync(
                _list,
                It.IsAny<IEnumerable<Status>>(),
                User,
                It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Status>, string, CancellationToken>((_, statuses, _, _) =>
            {
                persistedStatuses = statuses.ToArray();
            })
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            RemoveStatuses = [new RemoveStatusOp("Active", "Closed")]
        };

        await executor.ExecuteAsync(_listId, User, plan, CancellationToken.None);

        Assert.That(mappedBag, Is.Not.Null);
        Assert.That(mappedBag!["status"], Is.EqualTo("Closed"));
        Assert.That(persistedStatuses, Is.Not.Null);
        Assert.That(persistedStatuses!.Any(s => s.Name == "Active"), Is.False);
    }
}
