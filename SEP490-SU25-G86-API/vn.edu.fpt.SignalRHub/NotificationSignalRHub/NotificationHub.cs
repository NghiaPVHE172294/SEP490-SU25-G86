using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SEP490_SU25_G86_API.vn.edu.fpt.SignalRHub.NotificationSignalRHub
{
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.User?.FindFirst("uid")?.Value;
            if (!string.IsNullOrEmpty(userIdStr))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userIdStr}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.User?.FindFirst("uid")?.Value;
            if (!string.IsNullOrEmpty(userIdStr))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userIdStr}");
            await base.OnDisconnectedAsync(exception);
        }


    }
}
