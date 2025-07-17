using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService
{
    public class TwilioOtpProvider : IOTPProvider
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _verifyServiceSid;


        public TwilioOtpProvider(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _verifyServiceSid = config["Twilio:VerifyServiceSid"];

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendSmsAsync(string phone, string _)
        {
            await VerificationResource.CreateAsync(
                to: FormatPhoneVN(phone),
                channel: "sms",
                pathServiceSid: _verifyServiceSid
            );
        }

        private string FormatPhoneVN(string phone)
        {
            if (phone.StartsWith("0"))
                return "+84" + phone.Substring(1);
            return phone;
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
    }
}
