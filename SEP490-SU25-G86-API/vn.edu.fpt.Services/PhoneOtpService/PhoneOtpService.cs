using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PhoneOtpRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public class PhoneOtpService : IPhoneOtpService
    {
        private readonly IPhoneOtpRepository _phoneOtpRepository;
        private readonly ICacheService _cache;
        private readonly IOTPProvider _otpProvider;
        public PhoneOtpService(IPhoneOtpRepository phoneOtpRepository, ICacheService cache, IOTPProvider otpProvider)
        {
            _phoneOtpRepository = phoneOtpRepository;
            _cache = cache;
            _otpProvider = otpProvider;
        }

        public async Task SendOTPAsync(string phone)
        {
            await _otpProvider.SendSmsAsync(phone, null);
        }

        public async Task<bool> VerifyOTPAsync(int userId, string phone, string otp)
        {
            // 1. Xác minh OTP bằng Twilio (vẫn dùng số điện thoại)
            var isValid = await _otpProvider.VerifyOtpAsync(phone, otp);
            if (!isValid)
                return false;

            // 2. Lấy user bằng ID để cập nhật số điện thoại
            var user = await _phoneOtpRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Phone = phone;
            await _phoneOtpRepository.UpdateAsync(user);

            return true;
        }
    }
}
