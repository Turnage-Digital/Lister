using Lister.Core.Domain;
using Lister.Lists.Application.Endpoints.DeleteListItem;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class DeleteListItemCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IDomainEventQueue>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        _itemsStore = new Mock<IItemsStore<ItemDb>>();

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(_itemsStore.Object);

        var bagValidator = new Mock<IValidateListItemBag<ListDb>>();
        bagValidator.Setup(v => v.ValidateAsync(It.IsAny<ListDb>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object, bagValidator.Object);
        _handler = new DeleteListItemCommandHandler<ListDb, ItemDb>(_listsAggregate);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private DeleteListItemCommandHandler<ListDb, ItemDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;
    private Mock<IItemsStore<ItemDb>> _itemsStore;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new DeleteListItemCommand(Guid.NewGuid(), 0);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new DeleteListItemCommand(Guid.NewGuid(), 0)
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
    public void Handle_ThrowsInvalidOperationException_WhenItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteListItemCommand(Guid.NewGuid(), 0)
        {
            UserId = "user"
        };
        var list = new ListDb();

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _itemsStore.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemDb?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_DeletesItemSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new DeleteListItemCommand(Guid.NewGuid(), 0)
        {
            UserId = "user"
        };
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        var item = new ItemDb();

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _itemsStore.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _itemsStore.Setup(x => x.DeleteAsync(item, command.UserId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _itemsStore.Verify(x => x.DeleteAsync(item, command.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
