using System.Reflection;
using System.Text.Json;
using Lister.App.Server.Services;
using Lister.Core.Domain;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Domain.ValueObjects;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Entities;
using Lister.Lists.Infrastructure.Sql.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Lister.App.Server.Tests;

[TestFixture]
public class ListMigrationJobRunnerTests
{
    private static ListsDbContext CreateListsDbContext()
    {
        var options = new DbContextOptionsBuilder<ListsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ListsDbContext(options);
    }

    private static ListDb CreateList(Guid id, string name)
    {
        return new ListDb
        {
            Id = id,
            Name = name,
            Columns =
            {
                new ColumnDb { StorageKey = "prop1", Name = "Title", Type = ColumnType.Text },
                new ColumnDb { StorageKey = "prop2", Name = "Legacy", Type = ColumnType.Text }
            },
            Statuses =
            {
                new StatusDb { Name = "Active", Color = "green" },
                new StatusDb { Name = "Closed", Color = "gray" }
            },
            Items =
            {
                new ItemDb
                {
                    Id = 1,
                    ListId = id,
                    Bag = new Dictionary<string, object?>
                    {
                        ["prop1"] = "Hello",
                        ["prop2"] = "legacy",
                        ["status"] = "Active"
                    }
                }
            }
        };
    }

    [Test]
    public async Task RunAsync_RenamesSource_CreatesDestination_AndCopiesItems()
    {
        await using var dbContext = CreateListsDbContext();
        var listId = Guid.NewGuid();
        var list = CreateList(listId, "Tasks");
        dbContext.Lists.Add(list);
        await dbContext.SaveChangesAsync();

        var mediator = new Mock<IMediator>();
        var domainEvents = new Mock<IDomainEventQueue>();
        domainEvents
            .Setup(q => q.Dequeue(It.IsAny<EventPhase>()))
            .Returns([]);
        var unitOfWork = new ListsUnitOfWork(dbContext, mediator.Object, domainEvents.Object);
        var runner = new ListMigrationJobRunner(unitOfWork, dbContext, mediator.Object,
            NullLogger<ListMigrationJobRunner>.Instance);

        var plan = new MigrationPlan
        {
            RemoveColumns = [new RemoveColumnOp("prop2", "drop")]
        };

        var job = new ListMigrationJobDb
        {
            Id = Guid.NewGuid(),
            SourceListId = listId,
            RequestedBy = "tester",
            PlanJson = JsonSerializer.Serialize(plan, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            Stage = ListMigrationJobStage.Pending,
            CorrelationId = Guid.NewGuid()
        };

        await runner.RunAsync(job, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(job.BackupListId, Is.EqualTo(listId));
            Assert.That(job.NewListId, Is.Not.Null);
            Assert.That(job.NewListId, Is.Not.EqualTo(job.BackupListId));
            Assert.That(job.BackupExpiresOn, Is.Not.Null);
        }

        var backup = await dbContext.Lists.SingleAsync(l => l.Id == listId);
        var destination = await dbContext.Lists.SingleAsync(l => l.Id == job.NewListId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(backup.Name, Does.Contain("backup"));
            Assert.That(destination.Name, Is.EqualTo("Tasks"));
        }

        var destinationItems = await dbContext.Items
            .Where(i => i.ListId == job.NewListId)
            .ToListAsync();

        Assert.That(destinationItems.Count, Is.EqualTo(1));
        var bag = destinationItems[0].Bag as Dictionary<string, object?>;
        Assert.That(bag, Is.Not.Null);
        Assert.That(bag!.ContainsKey("prop2"), Is.False);
        Assert.That(Convert.ToString(bag["prop1"]), Is.EqualTo("Hello"));

        mediator.Verify(m => m.Publish(It.IsAny<ListMigrationStartedIntegrationEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
        mediator.Verify(
            m => m.Publish(It.IsAny<ListMigrationCompletedIntegrationEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task CleanupExpiredBackupsAsync_RemovesBackupAndMarksJob()
    {
        await using var dbContext = CreateListsDbContext();
        var listId = Guid.NewGuid();
        var backupList = CreateList(listId, "Tasks (backup 20240101010101)");
        dbContext.Lists.Add(backupList);
        await dbContext.SaveChangesAsync();

        var mediator = new Mock<IMediator>();
        var domainEvents = new Mock<IDomainEventQueue>();
        domainEvents
            .Setup(q => q.Dequeue(It.IsAny<EventPhase>()))
            .Returns([]);
        var unitOfWork = new ListsUnitOfWork(dbContext, mediator.Object, domainEvents.Object);

        var job = new ListMigrationJobDb
        {
            Id = Guid.NewGuid(),
            SourceListId = listId,
            RequestedBy = "tester",
            CorrelationId = Guid.NewGuid(),
            Stage = ListMigrationJobStage.Completed,
            BackupListId = listId,
            BackupExpiresOn = DateTime.UtcNow.AddMinutes(-5),
            PlanJson = JsonSerializer.Serialize(new MigrationPlan(),
                new JsonSerializerOptions(JsonSerializerDefaults.Web))
        };
        dbContext.ListMigrationJobs.Add(job);
        await dbContext.SaveChangesAsync();

        var cleanupMethod = typeof(ListMigrationDispatcherService)
            .GetMethod("CleanupExpiredBackupsAsync", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(cleanupMethod, Is.Not.Null);

        await (Task)cleanupMethod!.Invoke(null, [
            dbContext,
            unitOfWork,
            mediator.Object,
            NullLogger<ListMigrationDispatcherService>.Instance,
            CancellationToken.None
        ])!;

        await dbContext.Entry(job).ReloadAsync();
        await dbContext.Entry(backupList).ReloadAsync();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(job.Stage, Is.EqualTo(ListMigrationJobStage.Archived));
            Assert.That(job.BackupRemovedOn, Is.Not.Null);
            Assert.That(backupList.IsDeleted, Is.True);
        }

        mediator.Verify(m => m.Publish(It.Is<ListMigrationProgressIntegrationEvent>(evt =>
            evt.Message.Contains("Backup list removed")), It.IsAny<CancellationToken>()), Times.Once);
    }
}