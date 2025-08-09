namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO
{
    public record CreateNotificationRequest(
        long ReceiverUserId,
        string Content,
        string? TargetUrl = null
    );
}
