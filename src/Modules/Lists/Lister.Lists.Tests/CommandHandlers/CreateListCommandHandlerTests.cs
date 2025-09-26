using AutoMapper;
using Lister.Lists.Application.Endpoints.CreateList;
using Lister.Lists.Domain;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests.CommandHandlers;

[TestFixture]
public class CreateListCommandHandlerTests
{
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
        _mapper = new Mock<IMapper>();
        _handler = new CreateListCommandHandler<ListDb, ItemDb>(_listsAggregate, _mapper.Object);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _unitOfWork;
    private Mock<IMediator> _mediator;
    private ListsAggregate<ListDb, ItemDb> _listsAggregate;
    private Mock<IMapper> _mapper;
    private CreateListCommandHandler<ListDb, ItemDb> _handler;

    [Test]
    public void Handle_ThrowsArgumentNullException_WhenUserIdIsNull()
    {
        // Arrange
        var command = new CreateListCommand("Sample List", [], []);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_CreatesListSuccessfully_WhenValidRequest()
    {
        // Arrange
        var command = new CreateListCommand("Sample List", [], [])
        {
            UserId = "user"
        };
        var listItemDefinition = new ListItemDefinition();

        _mapper.Setup(x => x.Map<ListItemDefinition>(It.IsAny<IWritableList>()))
            .Returns(listItemDefinition);

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