using MediatR;

namespace Lister.Core.Domain;

public interface IDomainEventQueue
{
    void Enqueue(INotification @event, EventPhase phase);
    IReadOnlyCollection<INotification> Dequeue(EventPhase phase);
}