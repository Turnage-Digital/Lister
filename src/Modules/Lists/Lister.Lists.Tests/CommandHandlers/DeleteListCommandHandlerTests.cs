using Lister.Lists.Application.Endpoints.DeleteList;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class DeleteListCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IMediator>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object);
        _handler = new DeleteListCommandHandler<ListDb, ItemDb>(_listsAggregate);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IMediator> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private DeleteListCommandHandler<ListDb, ItemDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid().ToString());

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid().ToString())
        {
            UserId = "user"
        };

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ListDb?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_DeletesListSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new DeleteListCommand(Guid.NewGuid().ToString())
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