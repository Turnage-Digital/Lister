using Lister.Core.Domain;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Services;
using Lister.Lists.Domain.Views;
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

        var listsStore = new Mock<IListsStore<ListDb>>();
        var itemsStore = new Mock<IItemsStore<ItemDb>>();

        // Ensure created list has an Id like real stores would
        listsStore
            .Setup(x => x.InitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, string createdBy, CancellationToken _) =>
                new ListDb { Id = Guid.NewGuid(), Name = name });

        _unitOfWork.SetupGet(x => x.ListsStore).Returns(listsStore.Object);
        _unitOfWork.SetupGet(x => x.ItemsStore).Returns(itemsStore.Object);

        _listsAggregate = new ListsAggregate<ListDb, ItemDb>(_unitOfWork.Object, _mediator.Object);
        _definitionGetter = new Mock<IGetListItemDefinition>();
        _handler = new CreateListCommandHandler<ListDb, ItemDb>(_listsAggregate, _definitionGetter.Object);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IDomainEventQueue> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private Mock<IGetListItemDefinition> _definitionGetter;
    private CreateListCommandHandler<ListDb, ItemDb> _handler;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new CreateListCommand("Sample List", [], [], null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_CreatesListSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new CreateListCommand("Sample List", [], [], null)
        {
            UserId = "user"
        };
        var listItemDefinition = new ListItemDefinition();
        _definitionGetter
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(listItemDefinition);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(listItemDefinition));
        });
    }
}
