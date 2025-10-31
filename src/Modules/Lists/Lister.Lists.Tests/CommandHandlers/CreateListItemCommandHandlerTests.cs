using Lister.Core.Domain;
using Lister.Lists.Application.Endpoints.Commands.CreateListItem;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class CreateListItemCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IDomainEventQueue>();

        _listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();
        itemsStore
            .Setup(x => x.InitAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid listId, string createdBy, CancellationToken _) =>
                new ItemDb { ListId = listId });
        itemsStore
            .Setup(x => x.SetBagAsync(
                It.IsAny<ItemDb>(),
                It.IsAny<object>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback<ItemDb, object, string, CancellationToken>((item, bag, _, _) => item.Bag = bag)
            .Returns(Task.CompletedTask);
        itemsStore
            .Setup(x => x.CreateAsync(It.IsAny<ItemDb>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        itemsStore
            .Setup(x => x.GetBagAsync(It.IsAny<ItemDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemDb item, CancellationToken _) => item.Bag);

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(_listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var bagValidator = new Mock<IValidateListItemBag<ListDb>>();
        bagValidator.Setup(v => v.ValidateAsync(It.IsAny<ListDb>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object, bagValidator.Object);
        _handler = new CreateListItemCommandHandler<ListDb, ItemDb>(_listsAggregate);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private CreateListItemCommandHandler<ListDb, ItemDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new CreateListItemCommand(Guid.NewGuid(), new { });

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new CreateListItemCommand(Guid.NewGuid(), new { })
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
    public async Task Handle_CreatesListItemDtoSuccessfully_WhenValidRequest()
    {
        // Arrange
        var bag = new { name = "Sample" };
        var command = new CreateListItemCommand(Guid.NewGuid(), bag)
        {
            UserId = "user"
        };
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Bag, Is.EqualTo(bag));
        Assert.That(result.ListId, Is.EqualTo(list.Id));
        Assert.That(result.Id, Is.Null);
    }
}