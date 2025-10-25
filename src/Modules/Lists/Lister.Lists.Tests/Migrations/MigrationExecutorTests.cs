using Lister.Lists.Application.Endpoints.Migrations.RunMigration;
using Lister.Lists.Application.Endpoints.Migrations.Shared;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests.Migrations;

[TestFixture]
public class MigrationExecutorTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>>();
        _listsStore = new Mock<IListsStore<ListDb>>();
        _itemsStore = new Mock<IItemsStore<ItemDb>>();
        _itemStream = new Mock<IGetListItemStream>();
        _itemCount = new Mock<IGetListItemCount>();

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(_itemsStore.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

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

        _itemsStore.Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _itemCount.Setup(x => x.CountAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _itemStream.Setup(x => x.StreamAsync(_listId, It.IsAny<CancellationToken>()))
            .Returns((Guid _, CancellationToken _) => EmptyAsyncSequence());
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>> _unitOfWork = null!;
    private Mock<IListsStore<ListDb>> _listsStore = null!;
    private Mock<IItemsStore<ItemDb>> _itemsStore = null!;
    private Mock<IGetListItemStream> _itemStream = null!;
    private Mock<IGetListItemCount> _itemCount = null!;
    private ListDb _list = null!;
    private Guid _listId;
    private const string User = "tester";

    private MigrationExecutor<ListDb, ItemDb, ListMigrationJobDb> CreateExecutor()
    {
        return new MigrationExecutor<ListDb, ItemDb, ListMigrationJobDb>(
            _unitOfWork.Object,
            _itemStream.Object,
            _itemCount.Object);
    }

    [Test]
    public async Task ExecuteAsync_TightenConstraints_UpdatesMetadata()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = false }
            ]);

        Column[]? persisted = null;
        _listsStore.Setup(x => x.SetColumnsAsync(
                _list,
                It.IsAny<IEnumerable<Column>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Column>, string, CancellationToken>((_, cols, _, _) =>
            {
                persisted = cols.ToArray();
            })
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            TightenConstraints = [new TightenConstraintsOp("prop1", true, null, null, null, null)]
        };

        await executor.ExecuteAsync(
            _listId,
            User,
            plan,
            (_, _) => Task.CompletedTask,
            CancellationToken.None);

        Assert.That(persisted, Is.Not.Null);
        Assert.That(persisted!.Single().Required, Is.True);
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
                It.IsAny<string>(),
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

        await executor.ExecuteAsync(
            _listId,
            User,
            plan,
            (_, _) => Task.CompletedTask,
            CancellationToken.None);

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

        _itemCount.Setup(x => x.CountAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _itemStream.Setup(x => x.StreamAsync(_listId, It.IsAny<CancellationToken>()))
            .Returns((Guid _, CancellationToken _) => AsyncSequence(1));

        var item = new ItemDb { Id = 1, ListId = _listId };
        _itemsStore.Setup(x => x.GetByIdAsync(1, _listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, object?> { ["prop1"] = "42" });

        Dictionary<string, object?>? converted = null;
        _itemsStore.Setup(x => x.SetBagAsync(item, It.IsAny<object>(), User, It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((_, bag, _, _) =>
                converted = bag as Dictionary<string, object?>)
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            ChangeColumnTypes = [new ChangeColumnTypeOp("prop1", ColumnType.Number, "parseNumber")]
        };

        await executor.ExecuteAsync(
            _listId,
            User,
            plan,
            (_, _) => Task.CompletedTask,
            CancellationToken.None);

        Assert.That(converted, Is.Not.Null);
        Assert.That(converted!["prop1"], Is.TypeOf<decimal>());
    }

    [Test]
    public async Task ExecuteAsync_RemoveColumns_RemovesBagEntries()
    {
        _listsStore.Setup(x => x.GetColumnsAsync(_list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true },
                new Column { StorageKey = "prop2", Name = "Obsolete", Type = ColumnType.Text, Required = false }
            ]);

        _itemCount.Setup(x => x.CountAsync(_listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _itemStream.Setup(x => x.StreamAsync(_listId, It.IsAny<CancellationToken>()))
            .Returns((Guid _, CancellationToken _) => AsyncSequence(1));

        var item = new ItemDb { Id = 1, ListId = _listId };
        _itemsStore.Setup(x => x.GetByIdAsync(1, _listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, object?>
            {
                ["prop1"] = "hello",
                ["prop2"] = "remove"
            });

        Dictionary<string, object?>? bagAfterRemoval = null;
        _itemsStore.Setup(x => x.SetBagAsync(item, It.IsAny<object>(), User, It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((_, bag, _, _) =>
                bagAfterRemoval = bag as Dictionary<string, object?>)
            .Returns(Task.CompletedTask);

        var executor = CreateExecutor();
        var plan = new MigrationPlan
        {
            RemoveColumns = [new RemoveColumnOp("prop2", "drop")]
        };

        await executor.ExecuteAsync(
            _listId,
            User,
            plan,
            (_, _) => Task.CompletedTask,
            CancellationToken.None);

        Assert.That(bagAfterRemoval, Is.Not.Null);
        Assert.That(bagAfterRemoval!.ContainsKey("prop2"), Is.False);
    }

    private static async IAsyncEnumerable<int> AsyncSequence(params int[] values)
    {
        foreach (var value in values)
        {
            yield return value;
            await Task.Yield();
        }
    }

    private static async IAsyncEnumerable<int> EmptyAsyncSequence()
    {
        await Task.CompletedTask;
        yield break;
    }
}