using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.LoginDTO;
using FluentAssertions;

namespace CVMatcher_IntegrationTesting.vn.edu.fpt.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<SEP490_SU25_G86_API.Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(WebApplicationFactory<SEP490_SU25_G86_API.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "Đăng nhập thành công với tài khoản hợp lệ")]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var loginRequest = new LoginRequest {
                Email = "huythi.nk@gmail.com", // Cập nhật email test hợp lệ
                Password = "Huy123"           // Cập nhật mật khẩu test hợp lệ
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be(loginRequest.Email);
        }

        [Fact(DisplayName = "Đăng nhập thất bại với mật khẩu sai")]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest {
                Email = "huythi.nk@gmail.com", // Cập nhật email test hợp lệ
                Password = "Huy999999"      // Mật khẩu sai
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "Đăng nhập thất bại với tài khoản bị ban")]
        public async Task Login_BannedUser_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest {
                Email = "Employer123@gmail.com", // Cập nhật email bị ban
                Password = "Test123456"             // Mật khẩu hợp lệ
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("bị khóa");
        }

        /// <summary>
        /// Đăng nhập thất bại với email không tồn tại trong hệ thống
        /// </summary>
        [Fact(DisplayName = "Đăng nhập thất bại với email không tồn tại")]
        public async Task Login_NonExistentEmail_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest {
                Email = "notfound999@example.com", // Email chắc chắn không tồn tại
                Password = "Huy123"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Đăng nhập thất bại khi thiếu thông tin (email hoặc password rỗng/null)
        /// </summary>
        [Fact(DisplayName = "Đăng nhập thất bại khi thiếu thông tin")]
        public async Task Login_MissingFields_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest {
                Email = "", // Thiếu email
                Password = ""
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            // Có thể là BadRequest hoặc Unauthorized tùy backend, nên kiểm tra cả 2
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.BadRequest || x == System.Net.HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Đăng nhập thất bại với email chưa xác thực (nếu hệ thống có logic xác thực email)
        /// </summary>
        [Fact(DisplayName = "Đăng nhập thất bại với email chưa xác thực (nếu có)")]
        public async Task Login_UnverifiedEmail_ReturnsUnauthorizedOrForbidden()
        {
            // Nếu hệ thống không có logic xác thực email, có thể bỏ qua hoặc cập nhật test này
            var loginRequest = new LoginRequest {
                Email = "unverifieduser@example.com", // Cập nhật email test phù hợp
                Password = "AnyPassword"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            // Có thể trả về Unauthorized hoặc Forbidden tuỳ backend
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.Unauthorized || x == System.Net.HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Đăng nhập nhiều lần liên tiếp với tài khoản bị khóa, kiểm tra thông báo rõ ràng
        /// </summary>
        [Fact(DisplayName = "Đăng nhập nhiều lần với tài khoản bị khóa")]
        public async Task Login_BannedUser_MultipleAttempts_ShouldAlwaysReturnUnauthorized()
        {
            var loginRequest = new LoginRequest {
                Email = "Employer123@gmail.com", // Email bị ban
                Password = "Test123456"
            };
            for (int i = 0; i < 3; i++)
            {
                var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Contain("bị khóa");
            }
        }

        /// <summary>
        /// Đăng nhập thành công kiểm tra đầy đủ các trường trả về trong response
        /// </summary>
        [Fact(DisplayName = "Đăng nhập thành công kiểm tra các trường response")]
        public async Task Login_ValidCredentials_ResponseFieldsCorrect()
        {
            var loginRequest = new LoginRequest {
                Email = "huythi.nk@gmail.com", // Email hợp lệ
                Password = "Huy123"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be(loginRequest.Email);
            // Kiểm tra các trường khác nếu có logic (Role, UserId...)
            result.Role.Should().NotBeNullOrEmpty();
            result.UserId.Should().NotBeNull();
        }

        // Thêm các test case khác nếu cần thiết

        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
            public int? UserId { get; set; }
        }
    }
}
