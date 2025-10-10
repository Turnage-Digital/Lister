using Lister.Core.Domain;
using Lister.Lists.Domain;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Moq;

namespace Lister.Lists.Tests;

[TestFixture]
public class StatusTransitionTests
{
    [SetUp]
    public void SetUp()
    {
        _uow = new Mock<IListsUnitOfWork<ListDb, ItemDb>>();
        _lists = new Mock<IListsStore<ListDb>>();
        _items = new Mock<IItemsStore<ItemDb>>();
        _uow.SetupGet(x => x.ListsStore).Returns(_lists.Object);
        _uow.SetupGet(x => x.ItemsStore).Returns(_items.Object);
        _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var bagValidator = new ListItemBagValidator<ListDb, ItemDb>(_uow.Object);
        _agg = new ListsAggregate<ListDb, ItemDb>(_uow.Object, new NoopQueue(), bagValidator);
    }

    private Mock<IListsUnitOfWork<ListDb, ItemDb>> _uow = null!;
    private Mock<IListsStore<ListDb>> _lists = null!;
    private Mock<IItemsStore<ItemDb>> _items = null!;
    private ListsAggregate<ListDb, ItemDb> _agg = null!;

    private class NoopQueue : IDomainEventQueue
    {
        public void Enqueue(INotification @event, EventPhase phase)
        {
        }

        public IReadOnlyCollection<INotification> Dequeue(EventPhase phase)
        {
            return [];
        }
    }

    [Test]
    public void UpdateItem_Allows_Configured_Transition()
    {
        var list = new ListDb { Id = Guid.NewGuid() };
        var item = new ItemDb();
        var oldBag = new Dictionary<string, object?> { ["status"] = "Active" };
        var newBag = new Dictionary<string, object?> { ["status"] = "Done" };

        _items.Setup(s => s.GetBagAsync(item, It.IsAny<CancellationToken>())).ReturnsAsync(oldBag);
        _lists.Setup(s => s.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Status { Name = "Active" },
                new Status { Name = "Done" }
            ]);
        _lists.Setup(s => s.GetStatusTransitionsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new StatusTransition { From = "Active", AllowedNext = ["Done"] }]);

        Assert.DoesNotThrowAsync(async () => await _agg.UpdateItemAsync(list, item, newBag, "tester"));
    }

    [Test]
    public void UpdateItem_Blocks_Disallowed_Transition()
    {
        var list = new ListDb { Id = Guid.NewGuid() };
        var item = new ItemDb();
        var oldBag = new Dictionary<string, object?> { ["status"] = "Active" };
        var newBag = new Dictionary<string, object?> { ["status"] = "Backlog" };

        _items.Setup(s => s.GetBagAsync(item, It.IsAny<CancellationToken>())).ReturnsAsync(oldBag);
        _lists.Setup(s => s.GetStatusesAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Status { Name = "Active" },
                new Status { Name = "Done" },
                new Status { Name = "Backlog" }
            ]);
        _lists.Setup(s => s.GetStatusTransitionsAsync(list, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new StatusTransition { From = "Active", AllowedNext = ["Done"] }]);

        Assert.That(async () => await _agg.UpdateItemAsync(list, item, newBag, "tester"), Throws.Exception);
    }
}
