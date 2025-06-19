using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.LoginDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;

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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var account = _accountService.Authenticate(request.Email, request.Password);
            if (account == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
            if (account.Role == null)
                return Unauthorized(new { message = "Tài khoản chưa được gán quyền." });

            var roleName = account.Role.RoleName;
            var token = GenerateJwtToken(account, roleName);
            var user = _context.Users.FirstOrDefault(u => u.AccountId == account.AccountId);
            return Ok(new { token, role = roleName, email = account.Email, userId = user?.UserId });
        }

        private string GenerateJwtToken(Account account, string roleName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
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
                IsActive = true,
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
            return Ok();
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
            return Ok(new { token, role = roleName, email = account.Email, userId = account.AccountId });
        }
    }
}