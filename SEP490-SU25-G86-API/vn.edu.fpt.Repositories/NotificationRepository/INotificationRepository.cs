using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.NotificationRepository
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification n, CancellationToken ct = default);
        Task MarkReadAsync(long notificationId, long receiverUserId, CancellationToken ct = default);
        Task<IReadOnlyList<Notification>> GetLatestAsync(long receiverUserId, int take = 50, CancellationToken ct = default);
    }
}
