using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.EmailDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.RemindService
{
    public interface IRemindService
    {
        Task SendReminderAsync(ReminderEmailRequestDTO request);
    }
}
