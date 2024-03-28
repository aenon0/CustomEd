using AutoMapper;
using CustomEd.Contracts.Announcements.Events;
using CustomEd.Contracts.Notification.Events;

namespace CustomEd.Announcement.Service.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Notification.Service.Models.Notification, NotificationCreatedEvent>().ReverseMap();
            
        }
    }
}