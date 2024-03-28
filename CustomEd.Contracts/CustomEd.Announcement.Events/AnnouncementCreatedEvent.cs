namespace CustomEd.Contracts.Announcements.Events;

public class AnnouncementCreatedEvent
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? attachements { get; set; }
    public Guid  ClassRoomId { get; set; }
    public DateTime TimeStamp { get; set; }
}
