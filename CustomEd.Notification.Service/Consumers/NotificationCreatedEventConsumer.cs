using AutoMapper;
using CustomEd.Contracts.Notification.Events;
using CustomEd.Notification.Service.Models;
using CustomEd.Shared.Data.Interfaces;
using MassTransit;
namespace CustomEd.Notification.Service.Consumers;

public class NotificationCreatedEventConsumer : IConsumer<NotificationCreatedEvent>
{
    private readonly IGenericRepository<Models.Notification> _notificationRepository;
    private readonly IGenericRepository<Models.StudentNotification> _studentNotificationRepository;
    private readonly IMapper _mapper;
    public NotificationCreatedEventConsumer(IGenericRepository<Models.Notification> notificationRepository, IGenericRepository<Models.StudentNotification> studentNotificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _studentNotificationRepository = studentNotificationRepository;
        _mapper = mapper;
        
    }
    public async Task Consume(ConsumeContext<NotificationCreatedEvent> context)
    {
        var notificationEvent = context.Message;
        var notification = _mapper.Map<Models.Notification>(notificationEvent);
        await _notificationRepository.CreateAsync(notification);
        foreach(var studentId in notificationEvent.RecieversId){
            var studentNotification = new StudentNotification{
                Id = Guid.NewGuid(),
                StudentId = studentId, 
                NotificationId = notification.Id
            };
            await _studentNotificationRepository.CreateAsync(studentNotification);
        }

    }
}
