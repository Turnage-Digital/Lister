using MediatR;

namespace Lister.Core.Domain.IntegrationEvents;

public interface IIntegrationEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}