namespace Lister.Notifications.Domain.Events;

public class AllNotificationsReadEvent : MediatR.INotification
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