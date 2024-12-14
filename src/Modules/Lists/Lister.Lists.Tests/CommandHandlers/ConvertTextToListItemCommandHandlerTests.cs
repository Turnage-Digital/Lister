using Lister.Core.Domain.Services;
using Lister.Lists.Application.Endpoints.ConvertTextToListItem;
using Lister.Lists.Domain;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class ConvertTextToListItemCommandHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _mediator = new Mock<IMediator>();
        _completedJsonGetter = new Mock<IGetCompletedJson>();
        _logger = new Mock<ILogger<ConvertTextToListItemCommandHandler<ListDb, ItemDb>>>();

        var listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object);
        _handler = new ConvertTextToListItemCommandHandler<ListDb, ItemDb>(_listsAggregate, _completedJsonGetter.Object,
            _logger.Object);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IMediator> _mediator;
    private Mock<IGetCompletedJson> _completedJsonGetter;
    private Mock<ILogger<ConvertTextToListItemCommandHandler<ListDb, ItemDb>>> _logger;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private ConvertTextToListItemCommandHandler<ListDb, ItemDb> _handler;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new ConvertTextToListItemCommand(Guid.NewGuid().ToString(), "sample text");

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ThrowsInvalidOperationException_WhenListDoesNotExist()
    {
        // Arrange
        var command = new ConvertTextToListItemCommand(Guid.NewGuid().ToString(), "sample text")
        {
            UserId = "user"
        };

        _unitOfWork.Setup(x => x.ListsStore.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ListDb?)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_ReturnsListItem_WhenValidRequest()
    {
        // Arrange
        var command = new ConvertTextToListItemCommand(Guid.NewGuid().ToString(), "sample text")
        {
            UserId = "user"
        };
        var list = new Mock<ListDb>();

        const string completedJson = "{ }";

        _unitOfWork.Setup(x => x.ListsStore.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list.Object);
        _unitOfWork.Setup(x => x.ListsStore.GetColumnsAsync(It.IsAny<ListDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _unitOfWork.Setup(x => x.ListsStore.GetStatusesAsync(It.IsAny<ListDb>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _completedJsonGetter.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(completedJson);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Bag, Is.Not.Null);
        });
    }
}