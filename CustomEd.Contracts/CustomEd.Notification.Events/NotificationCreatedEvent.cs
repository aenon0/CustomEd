using CustomEd.Shared.Model;

namespace CustomEd.Contracts.Notification.Events;

public class NotificationCreatedEvent
{
    public string Title {set; get;}
    public string Description {set; get;}
    public NotificationType NotificationType{set; get;}
    public Guid DataId {set;get;}
    public List<Guid> RecieversId {set;get;}
    public DateTime CreatedAt {set; get;}
}
