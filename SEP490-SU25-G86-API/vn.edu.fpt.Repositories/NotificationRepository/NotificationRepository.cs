using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public NotificationRepository(SEP490_G86_CvMatchContext context) => _context = context;

        public async Task<Notification> AddAsync(Notification n, CancellationToken ct = default)
        {
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync(ct);
            return n;
        }

        public async Task MarkReadAsync(long notificationId, long receiverUserId, CancellationToken ct = default)
        {
            var entity = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId && x.ReceiverUserId == receiverUserId, ct)
                ?? throw new KeyNotFoundException("Notification not found");

            if (!entity.IsRead)
            {
                entity.IsRead = true;
                entity.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IReadOnlyList<Notification>> GetLatestAsync(long receiverUserId, int take = 50, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(x => x.ReceiverUserId == receiverUserId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
