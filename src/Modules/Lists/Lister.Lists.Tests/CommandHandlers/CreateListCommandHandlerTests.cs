using Lister.Core.Domain;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class CreateListCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IDomainEventQueue>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();
        itemsStore
            .Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _listsStore
            .Setup(x => x.SetColumnsAsync(
                It.IsAny<ListDb>(),
                It.IsAny<IEnumerable<Column>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _listsStore
            .Setup(x => x.SetStatusesAsync(
                It.IsAny<ListDb>(),
                It.IsAny<IEnumerable<Status>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _listsStore
            .Setup(x => x.SetStatusTransitionsAsync(
                It.IsAny<ListDb>(),
                It.IsAny<IEnumerable<StatusTransition>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _listsStore
            .Setup(x => x.GetColumnsAsync(
                It.IsAny<ListDb>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Column>);
        _listsStore
            .Setup(x => x.GetStatusesAsync(
                It.IsAny<ListDb>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Status>);
        _listsStore
            .Setup(x => x.GetStatusTransitionsAsync(
                It.IsAny<ListDb>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<StatusTransition>);

        // Ensure created list has an Id like real stores would
        _listsStore
            .Setup(x => x.InitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, string createdBy, CancellationToken _) =>
                new ListDb { Id = Guid.NewGuid(), Name = name });

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var bagValidator = new Mock<IValidateListItemBag<ListDb>>();
        bagValidator.Setup(v => v.ValidateAsync(It.IsAny<ListDb>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object, bagValidator.Object);
        _handler = new CreateListCommandHandler<ListDb, ItemDb>(_listsAggregate);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private CreateListCommandHandler<ListDb, ItemDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new CreateListCommand("Sample List", Array.Empty<Status>(), Array.Empty<Column>(), null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_CreatesListSuccessfully_WhenValidRequest()
    {
        // Arrange
        var inputColumns =
            new[]
            {
                new Column { Name = "Task Name", Type = ColumnType.Text },
                new Column { Name = "Status", Type = ColumnType.Text, StorageKey = "status" }
            };
        var statuses =
            new[]
            {
                new Status { Name = "Open", Color = "#00FF00" }
            };
        var transitions =
            new[]
            {
                new StatusTransition { From = "Open", AllowedNext = ["Closed"] }
            };
        var normalizedColumns =
            new[]
            {
                new Column { Name = "Task Name", Type = ColumnType.Text, StorageKey = "taskName" },
                new Column { Name = "Status", Type = ColumnType.Text, StorageKey = "status" }
            };
        _listsStore
            .Setup(x => x.GetColumnsAsync(It.IsAny<ListDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(normalizedColumns);
        _listsStore
            .Setup(x => x.GetStatusesAsync(It.IsAny<ListDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);
        _listsStore
            .Setup(x => x.GetStatusTransitionsAsync(It.IsAny<ListDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transitions);
        var command = new CreateListCommand("Sample List", statuses, inputColumns, transitions)
        {
            UserId = "user"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(command.Name));
            Assert.That(result.Columns, Has.Length.EqualTo(normalizedColumns.Length));
            Assert.That(result.Statuses, Has.Length.EqualTo(statuses.Length));
            Assert.That(result.Transitions, Has.Length.EqualTo(transitions.Length));
            Assert.That(result.Columns[0].StorageKey, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Columns[0].Property, Is.EqualTo("taskName"));
            Assert.That(result.Statuses[0].Color, Is.EqualTo("#00FF00"));
            Assert.That(result.Transitions[0].AllowedNext, Is.EquivalentTo(["Closed"]));
        });
    }
}
