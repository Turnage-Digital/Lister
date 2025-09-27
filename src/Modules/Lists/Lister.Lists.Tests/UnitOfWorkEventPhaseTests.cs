using Lister.Core.Domain;
using Lister.Core.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Lister.Lists.Tests;

[TestFixture]
public class UnitOfWorkEventPhaseTests
{
    private class BeforeEvent : INotification;

    private class AfterEvent : INotification;

    [Test]
    public async Task Publishes_BeforeSave_Then_AfterSave_In_Order()
    {
        var options = new DbContextOptionsBuilder<ListsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var ctx = new ListsDbContext(options);

        var published = new List<string>();
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Callback<INotification, CancellationToken>((e, _) => published.Add(e.GetType().Name))
            .Returns(Task.CompletedTask);

        var queue = new DomainEventQueue();
        queue.Enqueue(new BeforeEvent(), EventPhase.BeforeSave);
        queue.Enqueue(new AfterEvent(), EventPhase.AfterSave);

        using var uow = new ListsUnitOfWork(ctx, mediator.Object, queue);
        await uow.SaveChangesAsync(CancellationToken.None);

        Assert.That(published, Is.EqualTo([nameof(BeforeEvent), nameof(AfterEvent)]));
    }
}