using CustomEd.Shared.Model;

namespace CustomEd.Notification.Service.Models;

public class StudentNotification : BaseEntity
{
    public Guid StudentId {set; get;}
    public Guid NotificationId {set; get;}
    public bool IsRead {set; get;} = false;
}
