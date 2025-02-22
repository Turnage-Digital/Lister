using AutoMapper;
using Lister.Lists.Application.Commands.CreateListItem;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class CreateListItemCommandHandlerTests
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
        _mapper = new Mock<IMapper>();
        _handler = new CreateListItemCommandHandler<ListDb, ItemDb>(_listsAggregate, _mapper.Object);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IMediator> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private Mock<IMapper> _mapper;
    private CreateListItemCommandHandler<ListDb, ItemDb> _handler;
    private Mock<IListsStore<ListDb>> _listsStore;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new CreateListItemCommand(Guid.NewGuid().ToString(), new { });

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new CreateListItemCommand(Guid.NewGuid().ToString(), new { })
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
    public async Task Handle_CreatesListItemSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new CreateListItemCommand(Guid.NewGuid().ToString(), new { })
        {
            UserId = "user"
        };
        var list = new ListDb
        {
            Id = Guid.NewGuid()
        };
        var listItem = new ListItem();

        _listsStore.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
        _mapper.Setup(x => x.Map<ListItem>(It.IsAny<IWritableItem>()))
            .Returns(listItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(listItem));
    }
}