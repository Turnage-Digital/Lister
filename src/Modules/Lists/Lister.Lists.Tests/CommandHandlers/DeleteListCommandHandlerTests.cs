using Lister.Core.Domain;
using Lister.Lists.Application.Endpoints.DeleteList;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Queries;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class DeleteListCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>>();
        _mediator = new Mock<IDomainEventQueue>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();
        _itemStream = new Mock<IGetListItemStream>();
        _migrationJobGetter = new Mock<IGetListMigrationJob>();
        itemsStore
            .Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        var bagValidator = new Mock<IValidateListItemBag<ListDb>>();
        bagValidator.Setup(v => v.ValidateAsync(It.IsAny<ListDb>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb, ListMigrationJobDb>(
            _unitOfWork.Object,
            _mediator.Object,
            bagValidator.Object,
            _itemStream.Object,
            _migrationJobGetter.Object);
        _handler = new DeleteListCommandHandler<ListDb, ItemDb, ListMigrationJobDb>(_listsAggregate);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb, ListMigrationJobDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private ListsAggregate<ListDb, ItemDb, ListMigrationJobDb> _listsAggregate;
    private DeleteListCommandHandler<ListDb, ItemDb, ListMigrationJobDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;
    private Mock<IGetListItemStream> _itemStream = null!;
    private Mock<IGetListMigrationJob> _migrationJobGetter = null!;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid());

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid())
        {
            UserId = "user"
        };

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ListDb?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_DeletesListSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid())
        {
            UserId = "user"
        };
        var list = new ListDb();

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _listsStore.Verify(x => x.DeleteAsync(list, command.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}