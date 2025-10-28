using MediatR;

namespace Lister.Notifications.Domain.Events;

public class AllNotificationsReadEvent : INotification
{
    public AllNotificationsReadEvent(string userId, DateTime readOn, DateTime? before = null)
    {
        UserId = userId;
        ReadOn = readOn;
        Before = before;
    }

    public string UserId { get; }
    public DateTime ReadOn { get; }
    public DateTime? Before { get; }
}