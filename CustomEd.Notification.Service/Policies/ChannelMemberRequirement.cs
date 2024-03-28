using Microsoft.AspNetCore.Authorization;

namespace CustomEd.Notification.Service.Policies;

public class ChannelMemberRequirement  : IAuthorizationRequirement
{
    public Guid ChannelId { get; set;}
    public ChannelMemberRequirement(Guid channelId)
    {
        ChannelId = channelId;
    }
}
