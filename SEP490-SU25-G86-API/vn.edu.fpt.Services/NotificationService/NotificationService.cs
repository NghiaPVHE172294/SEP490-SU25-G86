    using Microsoft.AspNetCore.SignalR;
    using SEP490_SU25_G86_API.Models;
    using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO;
    using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.NotificationRepository;
    using SEP490_SU25_G86_API.vn.edu.fpt.SignalRHub.NotificationSignalRHub;

    namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService
    {
        public class NotificationService : INotificationService
        {
            private readonly INotificationRepository _repo;
            private readonly IHubContext<NotificationHub, INotificationClient> _hub;

            public NotificationService(INotificationRepository repo, IHubContext<NotificationHub, INotificationClient> hub)
            {
                _repo = repo;
                _hub = hub;
            }

            public async Task<NotificationResponse> SendAsync(CreateNotificationRequest req, CancellationToken ct = default)
            {
                if (string.IsNullOrWhiteSpace(req.Content))
                    throw new ArgumentException("Content is required");
                if (!string.IsNullOrWhiteSpace(req.TargetUrl) && req.TargetUrl.Length > 500)
                    throw new ArgumentException("TargetUrl too long");

                var entity = new Notification
                {
                    ReceiverUserId = (int)req.ReceiverUserId, // Explicit cast from long to int
                    Content = req.Content.Trim(),
                    TargetUrl = string.IsNullOrWhiteSpace(req.TargetUrl) ? null : req.TargetUrl.Trim()
                };

                entity = await _repo.AddAsync(entity, ct);

                var res = new NotificationResponse(
                    entity.NotificationId, entity.ReceiverUserId,
                    entity.Content, entity.TargetUrl, entity.IsRead, entity.CreatedAt);

                await _hub.Clients.Group($"user-{entity.ReceiverUserId}").ReceiveNotification(res);
                return res;
            }

            public async Task<IReadOnlyList<NotificationResponse>> GetLatestAsync(long receiverUserId, int take = 50, CancellationToken ct = default)
            {
                var items = await _repo.GetLatestAsync(receiverUserId, take, ct);
                return items.Select(x =>
                    new NotificationResponse(x.NotificationId, x.ReceiverUserId, x.Content, x.TargetUrl, x.IsRead, x.CreatedAt)
                ).ToList();
            }

        public async Task MarkReadAsync(long notificationId, long receiverUserId, CancellationToken ct = default)
        {
            await _repo.MarkReadAsync(notificationId, receiverUserId, ct);
            await _hub.Clients.Group($"user-{receiverUserId}")
                              .NotificationMarkedRead(notificationId);

            // (tuỳ chọn) tính lại số chưa đọc và bắn:
            // var cnt = await _repo.UnreadCountAsync(receiverUserId, ct);
            // await _hub.Clients.Group($"user-{receiverUserId}").UnreadCountChanged(cnt);
        }
    }
    }
