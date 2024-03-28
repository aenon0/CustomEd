using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace CustomEd.Notification.Service.Policies;
public class ChannelMemberAuthorizationHandler : AuthorizationHandler<ChannelMemberRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public ChannelMemberAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
        Console.WriteLine("been in the auth");
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ChannelMemberRequirement requirement)
    {
        Console.WriteLine("been in the auth");
        // var identityProvider = new IdentityProvider(_httpContextAccessor.HttpContext, _jwtService);
        // if (_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue("channelId", out object? channelIdValue)
        //         && Guid.TryParse(channelIdValue?.ToString(), out Guid channelId))
        // {
        //     // Console.WriteLine(identityProvider.GetUserId());
        //     var userId = identityProvider.GetUserId();
        //     requirement.ChannelId = channelId;
        //     Console.WriteLine("ChannelCreatorAuthorizationHandler");
        //     Console.WriteLine(channelId);
        //     Console.WriteLine(userId);
        //     foreach (var item in await _channelRepository.GetAllAsync())
        //     {
        //         Console.WriteLine(item.Id);
        //     }
        //     if (userId != null)
        //     {
        //         var channel = await _channelRepository.GetAsync(channelId);
        //         Console.WriteLine(channel);
        //         if (channel != null && channel.CreatorId == userId)
        //         {
                        context.Succeed(requirement);
        //         }
        //     }
        // }
        // throw new UnimplementedError();
        throw new NotImplementedException();
    }
}