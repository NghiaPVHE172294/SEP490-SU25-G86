namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO
{
    public record NotificationResponse(
        long NotificationId,
        long ReceiverUserId,
        string Content,
        string? TargetUrl,
        bool IsRead,
        System.DateTime CreatedAt
    );
}
