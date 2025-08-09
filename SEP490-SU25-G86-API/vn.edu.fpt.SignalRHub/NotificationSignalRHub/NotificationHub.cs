using Microsoft.AspNetCore.SignalR;

namespace SEP490_SU25_G86_API.vn.edu.fpt.SignalRHub.NotificationSignalRHub
{
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userIdStr))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userIdStr}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdStr = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userIdStr))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userIdStr}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
