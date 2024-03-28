using System.Security.Claims;
using CustomEd.Shared.Data.Interfaces;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CustomEd.Notification.Service.Hubs;

public class NotificationHub : Hub
{
    private readonly IGenericRepository<Models.Notification> _notificationRepository;
    private readonly IGenericRepository<Models.StudentNotification> _studentNotificationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public NotificationHub(IGenericRepository<Models.Notification> notificationRepository, IGenericRepository<Models.StudentNotification> studentNotificationRepository, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _notificationRepository = notificationRepository;
        _studentNotificationRepository = studentNotificationRepository;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var studentId = identityProvider.GetUserId();
            var studentNotifications = await _studentNotificationRepository.GetAllAsync(x => x.StudentId == studentId && x.IsRead == false);
            var notifications = new List<Models.Notification>();
            foreach (var studentNotification in studentNotifications)
            {
                var notificationId = studentNotification.NotificationId;
                var notification = await _notificationRepository.GetAsync(notificationId);   
                notifications.Add(notification);
            }
            var sortedNotifications = notifications.OrderBy(n => n.CreatedAt).ToList();
            await Clients.Caller.SendAsync("Notifications", sortedNotifications);
        }
        Console.WriteLine("Notifications sent");
        await base.OnConnectedAsync();
    }
        
    public async Task MarkAnnouncementAsSeen(Guid notificationId)
    {
        if (Context.GetHttpContext().Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var token = authorizationHeader.ToString().Replace("Bearer ", "");
            Console.WriteLine($"Access Token: {token}");
            var identityProvider = new IdentityProvider(_httpContextAccessor, _jwtService);
            var studentId = identityProvider.GetUserId();
            var studentNotification = await _studentNotificationRepository.GetAsync(x => x.StudentId == studentId && x.NotificationId == notificationId);
            if(studentNotification != null)
            {
                studentNotification.IsRead = true;
                await _studentNotificationRepository.UpdateAsync(studentNotification);
            }
        }
    }

}
