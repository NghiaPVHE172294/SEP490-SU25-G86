namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public interface IPhoneOtpService
    {
        Task SendOTPAsync(string phone);
        Task<bool> VerifyOTPAsync(int userId, string phone, string otp);
    }
}
