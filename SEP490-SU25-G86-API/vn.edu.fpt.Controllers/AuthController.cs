using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly SEP490_G86_CvMatchContext _context;

        public AuthController(AccountService accountService, IConfiguration configuration, SEP490_G86_CvMatchContext context)
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
            return Ok(new { token, role = roleName, email = account.Email });
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
            var account = new Account
            {
                Email = request.Email,
                Password = request.Password, // Đã mã hóa MD5 từ client
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

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string RoleName { get; set; }
        }
    }
} 