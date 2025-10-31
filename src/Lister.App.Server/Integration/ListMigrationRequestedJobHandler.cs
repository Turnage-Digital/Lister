using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Domain.Enums;
using Lister.Lists.Infrastructure.Sql;
using Lister.Lists.Infrastructure.Sql.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lister.App.Server.Integration;

public class ListMigrationRequestedJobHandler(
    ListsDbContext dbContext
) : INotificationHandler<ListMigrationRequestedIntegrationEvent>
{
    public async Task Handle(ListMigrationRequestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var existing = await dbContext.ListMigrationJobs
            .SingleOrDefaultAsync(j => j.CorrelationId == notification.CorrelationId, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var job = new ListMigrationJobDb
        {
            Id = Guid.NewGuid(),
            SourceListId = notification.ListId,
            RequestedBy = notification.RequestedBy,
            PlanJson = notification.PlanJson,
            CreatedOn = DateTime.UtcNow,
            Stage = ListMigrationJobStage.Pending,
            CorrelationId = notification.CorrelationId
        };

        dbContext.ListMigrationJobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}