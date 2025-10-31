using System.Text.Json;
using Lister.Core.Domain.IntegrationEvents;
using Lister.Lists.Application.Migrations.Services;
using Lister.Lists.Domain.Entities;
using MediatR;

namespace Lister.Lists.Application.Endpoints.Commands.Migrations.RunMigration;

public class RunMigrationCommandHandler<TList, TItem>(
    IMigrationValidator validator,
    IMediator mediator
) : IRequestHandler<RunMigrationCommand, MigrationResult>
    where TList : IWritableList
    where TItem : IWritableItem
{
    public async Task<MigrationResult> Handle(RunMigrationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null)
        {
            throw new ArgumentNullException(nameof(request), "request.UserId cannot be null");
        }

        var validation = await validator.ValidateAsync(request.ListId, request.Plan, cancellationToken);
        if (request.Mode == MigrationMode.DryRun || !validation.IsSafe)
        {
            return validation;
        }

        var correlationId = Guid.NewGuid();
        var planJson = JsonSerializer.Serialize(request.Plan);
        await mediator.Publish(
            new ListMigrationRequestedIntegrationEvent(request.ListId, correlationId, request.UserId, planJson),
            cancellationToken);

        var messages = validation.Messages.Length > 0
            ? validation.Messages
            : ["Migration queued."];

        return new MigrationResult
        {
            IsSafe = true,
            Messages = messages,
            SuggestedPlan = validation.SuggestedPlan,
            CorrelationId = correlationId
        };
    }
}