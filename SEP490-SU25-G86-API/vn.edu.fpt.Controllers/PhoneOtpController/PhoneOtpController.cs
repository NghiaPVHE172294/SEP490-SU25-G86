using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.PhoneOtpDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.PhoneOtpController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PhoneOtpController : Controller
    {
        private readonly IPhoneOtpService _otpService;

        public PhoneOtpController(IPhoneOtpService otpService)
        {
            _otpService = otpService;
        }

        // Gửi OTP tới số điện thoại
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] PhoneRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest("Phone number is required");

            await _otpService.SendOTPAsync(request.Phone);
            return Ok(new { message = "OTP sent successfully" });
        }

        // Xác minh OTP và lưu user vào DB nếu đúng
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OTPVerifyDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.OTP) || request.UserId <= 0)
                return BadRequest("UserId, phone and OTP are required");

            var result = await _otpService.VerifyOTPAsync(request.UserId, request.Phone, request.OTP);
            if (!result)
                return BadRequest(new { message = "OTP is invalid or expired" });

            return Ok(new { message = "Phone number verified and saved to user" });
        }

    }
}
