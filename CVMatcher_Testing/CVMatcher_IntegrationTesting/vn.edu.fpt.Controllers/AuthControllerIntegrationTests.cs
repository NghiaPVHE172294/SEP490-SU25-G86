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
        //Test Case cho Đăng nhập Thành công

        [Fact(DisplayName = "[TC_IT_LOGIN_01] Đăng nhập thành công với tài khoản hợp lệ")]
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

        [Fact(DisplayName = "[TC_IT_LOGIN_08] Đăng nhập thành công kiểm tra các trường response")]
        public async Task Login_ValidCredentials_ResponseFieldsCorrect()
        {
            var loginRequest = new LoginRequest
            {
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


        //Test Case cho Đăng nhập Thất bại (Email không hợp lệ)

        [Fact(DisplayName = "[TC_IT_LOGIN_09] Đăng nhập thất bại với email không đúng định dạng")]
        public async Task Login_InvalidEmailFormat_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "invalid-email-format", // Không đúng định dạng email
                Password = "Huy123"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.BadRequest || x == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "[TC_IT_LOGIN_10] Đăng nhập thất bại với email rỗng nhưng đúng mật khẩu")]
        public async Task Login_EmptyEmail_ValidPassword_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "",
                Password = "Huy123" // Đúng mật khẩu
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.BadRequest || x == System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "[TC_IT_LOGIN_05]Đăng nhập thất bại khi thiếu thông tin")]
        public async Task Login_MissingFields_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "", // Thiếu email
                Password = ""
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            // Có thể là BadRequest hoặc Unauthorized tùy backend, nên kiểm tra cả 2
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.BadRequest || x == System.Net.HttpStatusCode.Unauthorized);
        }




        //Test Case cho Đăng nhập Thất bại (Mật khẩu không hợp lệ)
        [Fact(DisplayName = "[TC_IT_LOGIN_11] Đăng nhập thất bại với email đúng nhưng mật khẩu rỗng")]
        public async Task Login_ValidEmail_EmptyPassword_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "huythi.nk@gmail.com", // Email hợp lệ
                Password = "" // Mật khẩu rỗng
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.BadRequest || x == System.Net.HttpStatusCode.Unauthorized);
        }


        //Test Case cho Đăng nhập Thất bại (Tài khoản không tồn tại hoặc sai mật khẩu)
        [Fact(DisplayName = "[TC_IT_LOGIN_04]Đăng nhập thất bại với email không tồn tại")]
        public async Task Login_NonExistentEmail_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest
            {
                Email = "notfound999@example.com", // Email chắc chắn không tồn tại
                Password = "Huy123"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "[TC_IT_LOGIN_02]Đăng nhập thất bại với mật khẩu sai")]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest
            {
                Email = "huythi.nk@gmail.com", // Cập nhật email test hợp lệ
                Password = "Huy999999"      // Mật khẩu sai
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }


        //Test Case cho Tình huống Biên (Edge Cases)
        [Fact(DisplayName = "[TC_IT_LOGIN_06]Đăng nhập thất bại với email chưa xác thực (nếu có)")]
        public async Task Login_UnverifiedEmail_ReturnsUnauthorizedOrForbidden()
        {
            // Nếu hệ thống không có logic xác thực email, có thể bỏ qua hoặc cập nhật test này
            var loginRequest = new LoginRequest
            {
                Email = "unverifieduser@example.com", // Cập nhật email test phù hợp
                Password = "AnyPassword"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            // Có thể trả về Unauthorized hoặc Forbidden tuỳ backend
            response.StatusCode.Should().Match(x => x == System.Net.HttpStatusCode.Unauthorized || x == System.Net.HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "[TC_IT_LOGIN_12] Đăng nhập thất bại với email có ký tự đặc biệt")]
        public async Task Login_EmailWithSpecialChars_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "huy~@fpt.com", // Email có ký tự đặc biệt
                Password = "Huy123"
            };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }



        //Test case cho tình huống đăng nhập thất lại với tài khoản bị ban

        [Fact(DisplayName = "[TC_IT_LOGIN_03]Đăng nhập thất bại với tài khoản bị ban")]
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


        [Fact(DisplayName = "[TC_IT_LOGIN_07] Đăng nhập nhiều lần với tài khoản bị khóa")]
        public async Task Login_BannedUser_MultipleAttempts_ShouldAlwaysReturnUnauthorized()
        {
            var loginRequest = new LoginRequest
            {
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





        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
            public int? UserId { get; set; }
        }
    }
}
