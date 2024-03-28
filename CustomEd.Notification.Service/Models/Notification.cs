using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service.Models;

public class Notification : BaseEntity
{
     public string Title {set; get;}
    public string Description {set; get;}
    public NotificationType NotificationType {set; get;}
    public Guid DataId {set;get;}
    public List<Guid> RecieversId {set;get;}
}
