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
                Email = "Testbanaccount1@gmail.com", // Cập nhật email bị ban
                Password = "Huy123"             // Mật khẩu hợp lệ
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("bị khóa");
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
