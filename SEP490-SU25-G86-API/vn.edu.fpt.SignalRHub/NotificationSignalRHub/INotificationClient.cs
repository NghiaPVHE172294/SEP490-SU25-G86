using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.SignalRHub.NotificationSignalRHub
{
    public interface INotificationClient
    {
        Task ReceiveNotification(NotificationResponse payload);
        Task NotificationMarkedRead(long notificationId);
    }
}
