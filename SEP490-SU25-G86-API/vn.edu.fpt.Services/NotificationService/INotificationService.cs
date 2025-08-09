using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService
{
    public interface INotificationService
    {
        Task<NotificationResponse> SendAsync(CreateNotificationRequest req, CancellationToken ct = default);
        Task<IReadOnlyList<NotificationResponse>> GetLatestAsync(long receiverUserId, int take = 50, CancellationToken ct = default);
        Task MarkReadAsync(long notificationId, long receiverUserId, CancellationToken ct = default);
    }
}
