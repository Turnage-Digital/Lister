using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests;

[TestFixture]
public class ListsAggregateTests
{
    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IMediator> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;

    const string BY = "heath";

    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IMediator>();

        var listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object);
    }

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

        _unitOfWork.Setup(x => x.ListsStore.GetColumnsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);
        _unitOfWork.Setup(x => x.ListsStore.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
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
}