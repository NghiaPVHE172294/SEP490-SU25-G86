using Microsoft.Extensions.Options;
using SEP490_SU25_G86_API.vn.edu.fpt.Helpers;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public class TwilioOtpProvider : IOTPProvider
    {
        private readonly string _verifyServiceSid;

        public TwilioOtpProvider(IOptions<TwilioSettings> options)
        {
            var settings = options.Value;
            var accountSid = settings.AccountSid;
            _verifyServiceSid = settings.VerifyServiceSid;

            // Lấy auth token từ biến môi trường
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            TwilioClient.Init(accountSid, authToken);
        }

        public async Task SendSmsAsync(string phone, string _)
        {
            await VerificationResource.CreateAsync(
                to: FormatPhoneVN(phone),
                channel: "sms",
                pathServiceSid: _verifyServiceSid
            );
        }

        public async Task<bool> VerifyOtpAsync(string phone, string otp)
        {
            var result = await VerificationCheckResource.CreateAsync(
                to: FormatPhoneVN(phone),
                code: otp,
                pathServiceSid: _verifyServiceSid
            );

            return result.Status == "approved";
        }

        private string FormatPhoneVN(string phone)
        {
            if (phone.StartsWith("0"))
                return "+84" + phone.Substring(1);
            return phone;
        }
    }
}
