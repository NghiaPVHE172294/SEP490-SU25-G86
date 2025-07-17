namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public interface IOTPProvider
    {
        Task SendSmsAsync(string phone, string message);
        Task<bool> VerifyOtpAsync(string phone, string otp);
    }
}
