using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.LoginDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AuthenticationController
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly SEP490_G86_CvMatchContext _context;

        public AuthController(IAccountService accountService, IConfiguration configuration, SEP490_G86_CvMatchContext context)
        {
            _accountService = accountService;
            _configuration = configuration;
            _context = context;
        }

        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginRequest request)
        //{
        //    var account = _accountService.Authenticate(request.Email, request.Password);
        //    if (account == null)
        //        return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
        //    if (account.Role == null)
        //        return Unauthorized(new { message = "Tài khoản chưa được gán quyền." });

        //    var roleName = account.Role.RoleName;
        //    var token = GenerateJwtToken(account, roleName);
        //    var user = _context.Users.FirstOrDefault(u => u.AccountId == account.AccountId);
        //    return Ok(new { token, role = roleName, email = account.Email, userId = user?.UserId });
        //}

        //cập nhật Login cho phần Ban User
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validate email format
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailValidator.IsValid(request.Email))
                return BadRequest("Email không đúng định dạng.");
            var account = _accountService.Authenticate(request.Email, request.Password);
            if (account == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });

            if (account.Role == null)
                return Unauthorized(new { message = "Tài khoản chưa được gán quyền." });

            // Chặn đăng nhập nếu chưa xác thực email
            if (account.IsActive == false)
                return Unauthorized(new { message = "Tài khoản chưa được xác thực email. Vui lòng kiểm tra email để xác thực tài khoản." });

            // ✅ Lấy user để kiểm tra IsBan
            var user = _context.Users.FirstOrDefault(u => u.AccountId == account.AccountId);
            if (user != null && user.IsBan == true)
                return Unauthorized(new { message = "Tài khoản của bạn đã bị khóa.\nVui lòng liên hệ quản trị viên." });

            var roleName = account.Role.RoleName;
            var token = GenerateJwtToken(account, roleName);

            return Ok(new { token, role = roleName, email = account.Email, userId = user?.UserId });
        }


        private string GenerateJwtToken(Account account, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]));

            //var claims = new[]
            //{
            //        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            //        new Claim(ClaimTypes.Role, roleName),

            //};

            // Lấy UserId theo AccountId
            var userId = _context.Users
                .Where(u => u.AccountId == account.AccountId)
                .Select(u => u.UserId)
                .FirstOrDefault();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
        new Claim(ClaimTypes.Role, roleName),
    };
            if (userId > 0) claims.Add(new Claim("uid", userId.ToString())); // thêm khi có

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("check-email")]
        public IActionResult CheckEmail([FromQuery] string email)
        {
            var account = _accountService.GetByEmail(email);
            return Ok(account != null);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Validate email format
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailValidator.IsValid(request.Email))
                return BadRequest("Email không đúng định dạng.");
            // Kiểm tra email đã tồn tại
            var existing = _accountService.GetByEmail(request.Email);
            if (existing != null)
                return BadRequest("Email đã tồn tại.");
            // Lấy role theo RoleName client gửi lên
            var role = _context.Roles.FirstOrDefault(r => r.RoleName == request.RoleName);
            if (role == null) return BadRequest($"Không tìm thấy role {request.RoleName}.");
            // Mã hóa password bằng MD5 ở backend
            string hashedPassword = AccountService.GetMd5HashStatic(request.Password);
            var account = new Account
            {
                Email = request.Email,
                Password = hashedPassword, // Đã mã hóa MD5 ở backend
                RoleId = role.RoleId,
                IsActive = false, // Chưa xác thực email
                CreatedDate = DateTime.Now
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();
            // Tạo user profile cơ bản
            var user = new User
            {
                FullName = request.FullName,
                AccountId = account.AccountId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Tạo token xác thực email, lưu vào PasswordResetToken
            var verifyToken = Guid.NewGuid().ToString();
            var expire = DateTime.Now.AddHours(2);
            var tokenEntity = new PasswordResetToken
            {
                AccountId = account.AccountId,
                Token = verifyToken,
                ExpireAt = expire,
                IsUsed = false,
                CreateAt = DateTime.Now
            };
            _context.PasswordResetTokens.Add(tokenEntity);
            _context.SaveChanges();

            // Gửi email xác thực cho user
            var verifyLink = $"https://localhost:7283/Common/VerifyEmail?token={verifyToken}";
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var smtpUser = emailSettings["SmtpUser"];
            var smtpPass = emailSettings["SmtpPass"];
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var mail = new MailMessage();
            mail.From = new MailAddress(senderEmail, senderName);
            mail.To.Add(request.Email);
            mail.Subject = "[CVMatcher] Xác thực tài khoản của bạn";
            mail.Body = $"Vui lòng nhấn vào link sau để xác thực tài khoản: {verifyLink}\n\nLink sẽ hết hạn sau 2 giờ.";
            mail.IsBodyHtml = false;
            using (var smtp = new SmtpClient(smtpServer, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            return Ok("Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.");
        }

        [HttpGet("verify-email")]
        public IActionResult VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Thiếu token xác thực.");
            var tokenEntity = _context.PasswordResetTokens.FirstOrDefault(t => t.Token == token);
            if (tokenEntity == null)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
            if (tokenEntity.IsUsed == true)
                return BadRequest("Token đã được sử dụng.");
            if (tokenEntity.ExpireAt < DateTime.Now)
                return BadRequest("Token đã hết hạn.");
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == tokenEntity.AccountId);
            if (account == null)
                return BadRequest("Tài khoản không tồn tại.");
            account.IsActive = true;
            tokenEntity.IsUsed = true;
            _context.SaveChanges();
            return Ok("Xác thực email thành công. Bạn có thể đăng nhập.");
        }

        [HttpPost("resend-verification-email")]
        public IActionResult ResendVerificationEmail([FromBody] string email)
        {
            // Kiểm tra email tồn tại
            var account = _accountService.GetByEmail(email);
            if (account == null)
                return NotFound("Email không tồn tại.");
            if (account.IsActive == true)
                return BadRequest("Tài khoản đã được xác thực email.");
            // Tạo token mới
            var verifyToken = Guid.NewGuid().ToString();
            var expire = DateTime.Now.AddHours(2);
            var tokenEntity = new PasswordResetToken
            {
                AccountId = account.AccountId,
                Token = verifyToken,
                ExpireAt = expire,
                IsUsed = false,
                CreateAt = DateTime.Now
            };
            _context.PasswordResetTokens.Add(tokenEntity);
            _context.SaveChanges();
            // Gửi email xác thực
            var verifyLink = $"https://localhost:7283/Common/VerifyEmail?token={verifyToken}";
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var smtpUser = emailSettings["SmtpUser"];
            var smtpPass = emailSettings["SmtpPass"];
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress(senderEmail, senderName);
            mail.To.Add(email);
            mail.Subject = "[CVMatcher] Gửi lại email xác thực tài khoản";
            mail.Body = $"Vui lòng nhấn vào link sau để xác thực tài khoản: {verifyLink}\n\nLink sẽ hết hạn sau 2 giờ.";
            mail.IsBodyHtml = false;
            using (var smtp = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
            {
                smtp.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            return Ok("Đã gửi lại email xác thực. Vui lòng kiểm tra hộp thư.");
        }

        [HttpPost("external-login/google")]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalLoginRequest request)
        {
            if (request.Provider != "Google" || string.IsNullOrEmpty(request.IdToken))
                return BadRequest("Invalid request");
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            }
            catch
            {
                return Unauthorized("Invalid Google token");
            }
            string googleEmail = payload.Email;
            string fullName = payload.Name ?? "Google User";
            var account = _accountService.GetByEmail(googleEmail);
            if (account == null)
            {
                // Tạo tài khoản mới cho user Google
                var role = _context.Roles.FirstOrDefault(r => r.RoleName == "CANDIDATE");
                account = new Account
                {
                    Email = googleEmail,
                    Password = "", // Không có password
                    RoleId = role?.RoleId ?? 2,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
                var user = new User
                {
                    FullName = fullName,
                    AccountId = account.AccountId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            var roleName = account.Role?.RoleName ?? "CANDIDATE";
            var token = GenerateJwtToken(account, roleName);
            var userEntity = _context.Users.FirstOrDefault(u => u.AccountId == account.AccountId);

            // ✅ LẤY userEntity và KIỂM TRA IsBan
            if (userEntity?.IsBan == true)
            {
                return Unauthorized("Tài khoản của bạn đã bị khóa.\nVui lòng liên hệ quản trị viên.");
            }

            return Ok(new { token, role = roleName, email = account.Email, userId = userEntity?.UserId });
        }

        [HttpPost("external-login/facebook")]
        public async Task<IActionResult> FacebookLogin([FromBody] ExternalLoginRequest request)
        {
            if (request.Provider != "Facebook" || string.IsNullOrEmpty(request.AccessToken))
                return BadRequest("Invalid request");
            // Gọi Facebook Graph API để lấy thông tin user
            using (var httpClient = new HttpClient())
            {
                var fbRes = await httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email&access_token={request.AccessToken}");
                if (!fbRes.IsSuccessStatusCode)
                    return Unauthorized("Invalid Facebook token");
                var fbContent = await fbRes.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(fbContent);
                var root = doc.RootElement;
                string facebookEmail = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
                string fullName = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Facebook User";
                string facebookId = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                // Nếu không có email, dùng id Facebook làm email giả
                if (string.IsNullOrEmpty(facebookEmail) && !string.IsNullOrEmpty(facebookId))
                {
                    facebookEmail = $"fbuser_{facebookId}@facebook.local";
                }
                if (string.IsNullOrEmpty(facebookEmail))
                    return BadRequest("Không lấy được thông tin định danh từ Facebook");
                var account = _accountService.GetByEmail(facebookEmail);
                if (account == null)
                {
                    // Tạo tài khoản mới cho user Facebook
                    var role = _context.Roles.FirstOrDefault(r => r.RoleName == "CANDIDATE");
                    account = new Account
                    {
                        Email = facebookEmail,
                        Password = "", // Không có password
                        RoleId = role?.RoleId ?? 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };
                    _context.Accounts.Add(account);
                    _context.SaveChanges();
                    var user = new User
                    {
                        FullName = fullName,
                        AccountId = account.AccountId,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }
                var roleName = account.Role?.RoleName ?? "CANDIDATE";
                var token = GenerateJwtToken(account, roleName);
                var userEntity = _context.Users.FirstOrDefault(u => u.AccountId == account.AccountId);

                // ✅ LẤY userEntity và KIỂM TRA IsBan
                if (userEntity?.IsBan == true)
                {
                    return Unauthorized("Tài khoản của bạn đã bị khóa.\nVui lòng liên hệ quản trị viên.");
                }

                return Ok(new { token, role = roleName, email = account.Email, userId = userEntity?.UserId });
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string email)
        {
            var account = _accountService.GetByEmail(email);
            if (account == null)
                return NotFound("Email không tồn tại.");
            // Tạo token reset
            var token = Guid.NewGuid().ToString();
            var expire = DateTime.Now.AddMinutes(30);
            var resetToken = new PasswordResetToken
            {
                AccountId = account.AccountId,
                Token = token,
                ExpireAt = expire,
                IsUsed = false,
                CreateAt = DateTime.Now
            };
            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();
            // Gửi email thật
            var resetLink = $"https://localhost:7283/Common/ResetPassword?token={token}";
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var smtpUser = emailSettings["SmtpUser"];
            var smtpPass = emailSettings["SmtpPass"];
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var mail = new MailMessage();
            mail.From = new MailAddress(senderEmail, senderName);
            mail.To.Add(email);
            mail.Subject = "Đặt lại mật khẩu CVMatcher";
            mail.Body = $"Nhấn vào link sau để đặt lại mật khẩu: {resetLink}";
            mail.IsBodyHtml = false;
            using (var smtp = new SmtpClient(smtpServer, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            return Ok("Đã gửi email đặt lại mật khẩu (nếu email tồn tại trong hệ thống).");
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest req)
        {
            // Validate strong password: 6 ký tự, chữ hoa, chữ thường, số
            var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");
            if (!regex.IsMatch(req.NewPassword))
                return BadRequest("Mật khẩu mới phải có ít nhất 1 chữ hoa, 1 chữ thường, 1 số và tối thiểu 6 ký tự.");

            var tokenEntity = _context.PasswordResetTokens.FirstOrDefault(t => t.Token == req.Token && t.ExpireAt > DateTime.Now && (t.IsUsed == false || t.IsUsed == null));
            if (tokenEntity == null)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == tokenEntity.AccountId);
            if (account == null)
                return BadRequest("Tài khoản không tồn tại.");
            account.Password = AccountService.GetMd5HashStatic(req.NewPassword);
            tokenEntity.IsUsed = true;
            _context.SaveChanges();
            return Ok("Đặt lại mật khẩu thành công.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                // Lấy accountId từ JWT
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (accountId == 0) return Unauthorized("Không thể xác định người dùng.");

                dto.AccountId = accountId;

                var success = await _accountService.ChangePasswordAsync(dto);
                return success ? Ok("Đổi mật khẩu thành công.") : BadRequest("Đổi mật khẩu thất bại.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class ResetPasswordRequest
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }
    }
}