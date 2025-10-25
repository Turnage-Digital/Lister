using Lister.Core.Domain;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.Events;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests;

[TestFixture]
public class ListsAggregateTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>>();
        _mediator = new Mock<IDomainEventQueue>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        _itemsStore = new Mock<IItemsStore<ItemDb>>();
        _itemStream = new Mock<IGetListItemStream>();
        _migrationJobGetter = new Mock<IGetListMigrationJob>();
        _itemsStore
            .Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(_itemsStore.Object);

        var bagValidator = new ListItemBagValidator<ListDb, ItemDb, ListMigrationJobDb>(_unitOfWork.Object);
        _listsAggregate = new ListsAggregate<ListDb, ItemDb, ListMigrationJobDb>(
            _unitOfWork.Object,
            _mediator.Object,
            bagValidator,
            _itemStream.Object,
            _migrationJobGetter.Object);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private Mock<IListsStore<ListDb>> _listsStore = null!;
    private Mock<IItemsStore<ItemDb>> _itemsStore = null!;
    private Mock<IGetListItemStream> _itemStream = null!;
    private Mock<IGetListMigrationJob> _migrationJobGetter = null!;
    private ListsAggregate<ListDb, ItemDb, ListMigrationJobDb> _listsAggregate;

    private const string BY = "heath";

    [Test]
    public async Task GetListByIdAsync_ReturnsList_WhenIdExists()
    {
        // Arrange
        var list = new ListDb();
        var id = Guid.NewGuid();

        _unitOfWork.Setup(x => x.ListsStore.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        var result = await _listsAggregate.GetListByIdAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(list));
        });
    }

    [Test]
    public async Task GetListByIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _unitOfWork.Setup(x => x.ListsStore.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ListDb?)null);

        // Act
        var result = await _listsAggregate.GetListByIdAsync(id);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateListAsync_CreatesAndReturnsList()
    {
        // Arrange
        var list = new ListDb();

        const string name = "Test List";

        var statuses = new List<Status> { new() { Name = "Active" } };
        var columns = new List<Column> { new() { Name = "Column1", Type = ColumnType.Text } };

        _unitOfWork.Setup(x => x.ListsStore.InitAsync(name, BY, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        var result = await _listsAggregate.CreateListAsync(BY, name, statuses, columns);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(list));
        });
    }

    [Test]
    public async Task DeleteListAsync_DeletesList()
    {
        // Arrange
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };

        // Act
        await _listsAggregate.DeleteListAsync(list, BY);

        // Assert
        Assert.Multiple(() =>
        {
            _unitOfWork.Verify(x => x.ListsStore.DeleteAsync(list, BY, It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Test]
    public async Task CreateItemAsync_CreatesAndReturnsItem()
    {
        // Arrange
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        var bag = new { };
        var item = new Mock<ItemDb>();

        _unitOfWork.Setup(x => x.ItemsStore.InitAsync(list.Id.Value, BY, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item.Object);

        // Act
        var result = await _listsAggregate.CreateItemAsync(list, bag, BY);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(item.Object));
        });
    }

    [Test]
    public void CreateItemAsync_Throws_WhenStatusNotInList()
    {
        // Arrange
        var list = new ListDb { Id = Guid.NewGuid() };
        var bag = new Dictionary<string, object?> { { "status", "Unknown" } };
        var columns = Array.Empty<Column>();
        var statuses = new[] { new Status { Name = "Active" } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);
        _listsStore.Setup(x => x.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act + Assert
        Assert.That(async () => await _listsAggregate.CreateItemAsync(list, bag, BY), Throws.Exception);
    }

    [Test]
    public void CreateItemAsync_Throws_WhenAllowedValuesViolated()
    {
        // Arrange
        var list = new ListDb { Id = Guid.NewGuid() };
        var bag = new Dictionary<string, object?> { { "priority", "Low" } };
        var columns = new[]
            { new Column { Name = "Priority", Type = ColumnType.Text, AllowedValues = ["High", "Medium"] } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);

        // Act + Assert
        Assert.That(async () => await _listsAggregate.CreateItemAsync(list, bag, BY), Throws.Exception);
    }

    [Test]
    public void CreateItemAsync_Throws_WhenRequiredTextEmpty()
    {
        // Arrange
        var list = new ListDb { Id = Guid.NewGuid() };
        var bag = new Dictionary<string, object?> { { "title", "" } };
        var columns = new[] { new Column { Name = "Title", Type = ColumnType.Text, Required = true } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);

        // Act + Assert
        Assert.That(async () => await _listsAggregate.CreateItemAsync(list, bag, BY), Throws.Exception);
    }

    [Test]
    public void CreateItemAsync_Throws_WhenNumberOutOfRange()
    {
        // Arrange
        var list = new ListDb { Id = Guid.NewGuid() };
        var bag = new Dictionary<string, object?> { { "score", 200 } };
        var columns = new[] { new Column { Name = "Score", Type = ColumnType.Number, MinNumber = 0, MaxNumber = 100 } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);

        // Act + Assert
        Assert.That(async () => await _listsAggregate.CreateItemAsync(list, bag, BY), Throws.Exception);
    }

    [Test]
    public void CreateItemAsync_Throws_WhenRegexNotMatched()
    {
        // Arrange
        var list = new ListDb { Id = Guid.NewGuid() };
        var bag = new Dictionary<string, object?> { { "email", "not-an-email" } };
        var columns = new[]
            { new Column { Name = "Email", Type = ColumnType.Text, Regex = @"^[^@\n]+@[^@\n]+\.[^@\n]+$" } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);

        // Act + Assert
        Assert.That(async () => await _listsAggregate.CreateItemAsync(list, bag, BY), Throws.Exception);
    }

    [Test]
    public async Task CreateItemsAsync_CreatesAndReturnsItems()
    {
        // Arrange
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        var bags = new List<object> { new { }, new { } };
        var items = new List<ItemDb> { new Mock<ItemDb>().Object, new Mock<ItemDb>().Object };

        _unitOfWork.SetupSequence(x => x.ItemsStore.InitAsync(list.Id.Value, BY, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items[0])
            .ReturnsAsync(items[1]);

        // Act
        var result = await _listsAggregate.CreateItemsAsync(list, bags, BY);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(items.Count));
            Assert.That(result, Is.EqualTo(items));
        });
    }

    [Test]
    public async Task UpdateItemAsync_EnqueuesListItemUpdatedEvent()
    {
        // Arrange
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        var item = new ItemDb
        {
            Id = 42,
            ListId = list.Id
        };

        var oldBag = new Dictionary<string, object?> { { "title", "old" } };
        var newBag = new Dictionary<string, object?> { { "title", "new" } };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Column>());
        _unitOfWork.Setup(x => x.ItemsStore.GetBagAsync(item, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldBag);
        _unitOfWork.Setup(x => x.ItemsStore.SetBagAsync(item, newBag, BY, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        await _listsAggregate.UpdateItemAsync(list, item, newBag, BY);

        // Assert
        _mediator.Verify(q => q.Enqueue(
                It.Is<ListItemUpdatedEvent>(evt =>
                    evt.Item == item &&
                    evt.UpdatedBy == BY &&
                    evt.PreviousBag == oldBag &&
                    evt.NewBag == newBag),
                EventPhase.AfterSave),
            Times.Once);
    }

    [Test]
    public async Task DeleteItemAsync_DeletesItem()
    {
        // Arrange
        var item = new ItemDb();

        // Act
        await _listsAggregate.DeleteItemAsync(item, BY);

        // Assert
        Assert.Multiple(() =>
        {
            _unitOfWork.Verify(x => x.ItemsStore.DeleteAsync(item, BY, It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Test]
    public async Task CreateExampleBagAsync_CreatesBagWithCorrectValues()
    {
        // Arrange
        var list = new ListDb();
        var columns = new[]
        {
            new Column { Name = "TextColumn", Type = ColumnType.Text },
            new Column { Name = "NumberColumn", Type = ColumnType.Number },
            new Column { Name = "DateColumn", Type = ColumnType.Date },
            new Column { Name = "BooleanColumn", Type = ColumnType.Boolean }
        };
        var statuses = new[]
        {
            new Status { Name = "Active" }
        };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);
        _listsStore.Setup(x => x.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _listsAggregate.CreateExampleBagAsync(list);

        // Assert
        var bag = (IDictionary<string, object?>)result;
        Assert.Multiple(() =>
        {
            Assert.That(bag["textColumn"], Is.EqualTo("text"));
            Assert.That(bag["numberColumn"], Is.EqualTo(77));
            Assert.That(bag["dateColumn"], Is.EqualTo(DateTime.Today.ToString("O")));
            Assert.That(bag["booleanColumn"], Is.EqualTo(true));
            Assert.That(bag["status"], Is.EqualTo("Active"));
        });
    }

    [Test]
    public async Task UpdateListAsync_AssignsStorageKeysToNewColumns()
    {
        var list = new ListDb { Id = Guid.NewGuid() };
        var existing = new[]
        {
            new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true }
        };

        _listsStore.Setup(x => x.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _listsStore.Setup(x => x.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Status>());
        _unitOfWork.Setup(x => x.ListsStore.GetStatusTransitionsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<StatusTransition>());

        Column[]? persisted = null;
        _listsStore.Setup(x =>
                x.SetColumnsAsync(list, It.IsAny<IEnumerable<Column>>(), BY, It.IsAny<CancellationToken>()))
            .Callback<ListDb, IEnumerable<Column>, string, CancellationToken>((_, cols, _, _) =>
            {
                persisted = cols.ToArray();
            })
            .Returns(Task.CompletedTask);

        var update = new[]
        {
            new Column { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text, Required = true },
            new Column { Name = "Priority", Type = ColumnType.Number }
        };

        await _listsAggregate.UpdateListAsync(list, update, null, null, BY, CancellationToken.None);

        Assert.That(persisted, Is.Not.Null);
        var priority = persisted!.Single(c => c.Name == "Priority");
        Assert.That(priority.StorageKey, Is.Not.Null.And.Not.Empty);
        Assert.That(priority.StorageKey, Is.Not.EqualTo("prop1"));
    }
}