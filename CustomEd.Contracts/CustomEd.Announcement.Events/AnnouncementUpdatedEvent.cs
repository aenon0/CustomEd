namespace CustomEd.Contracts.Announcements.Events;

public class AnnouncementUpdatedEvent
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<string>? attachements { get; set; }
    public Guid  ClassRoomId { get; set; }
}
