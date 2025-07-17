using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_Client.Pages.Admin
{
    public class ProfileOfUserForAdminModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProfileOfUserForAdminModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public UserDetailOfAdminDTO User { get; set; }

        [BindProperty]
        public string RemindMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int accountId)
        {
            if (accountId <= 0)
            {
                return RedirectToPage("/NotFound");
            }

            // Lấy token từ Session
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            // Gán token vào header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Gọi API
            var response = await _httpClient.GetAsync($"https://localhost:7004/api/UserForAdmin/GetUserByAccount/{accountId}");

            if (response.IsSuccessStatusCode)
            {
                User = await response.Content.ReadFromJsonAsync<UserDetailOfAdminDTO>();
                return Page();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Login"); // Token hết hạn hoặc sai, redirect về Login
            }

            return NotFound();
        }


        public async Task<IActionResult> OnPostAsync(int accountId)
        {
            if (string.IsNullOrWhiteSpace(RemindMessage))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập nội dung lời nhắc.");
                return await OnGetAsync(accountId);
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // GỌI LẠI API LẤY THÔNG TIN USER
            var responseUser = await _httpClient.GetAsync($"https://localhost:7004/api/UserForAdmin/GetUserByAccount/{accountId}");
            if (responseUser.IsSuccessStatusCode)
            {
                User = await responseUser.Content.ReadFromJsonAsync<UserDetailOfAdminDTO>();
            }
            else
            {
                TempData["ErrorMessage"] = "Không lấy được thông tin người dùng.";
                return await OnGetAsync(accountId);
            }

            if (string.IsNullOrWhiteSpace(User.AccountEmail))
            {
                TempData["ErrorMessage"] = "Email người dùng không hợp lệ.";
                return await OnGetAsync(accountId);
            }

            // Gửi mail
            var payload = new
            {
                toEmail = User.AccountEmail,
                subject = "Lời nhắc từ hệ thống CVMatcher",
                message = RemindMessage
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7004/api/AdminSendRemind/sendRemind", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã gửi lời nhắc thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Lỗi khi gửi lời nhắc.";
            }

            return await OnGetAsync(accountId);
        }

        public async Task<IActionResult> OnPostBanAsync(int accountId, int userId)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Gọi API BAN
            var response = await _httpClient.PostAsync(
                $"https://localhost:7004/api/UserForAdmin/BanUser/{userId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã cấm user thành công.";

                // GỌI LẠI API để lấy email
                var userResponse = await _httpClient.GetAsync($"https://localhost:7004/api/UserForAdmin/GetUserByAccount/{accountId}");
                if (userResponse.IsSuccessStatusCode)
                {
                    User = await userResponse.Content.ReadFromJsonAsync<UserDetailOfAdminDTO>();

                    string subject = "[CVMatcher] TÀI KHOẢN của bạn đã bị tạm KHOÁ";
                    string message = $@"
                    <p>Xin chào <b>{User.FullName ?? "người dùng"}</b>,</p>
                    <p>Tài khoản của bạn trên hệ thống <b>CVMatcher</b> đã bị <span style='color:red;'>tạm khóa</span> do vi phạm chính sách hoặc theo yêu cầu từ quản trị viên.</p>
                    <p>Nếu bạn cho rằng đây là sự nhầm lẫn hoặc cần hỗ trợ, vui lòng liên hệ với bộ phận hỗ trợ khách hàng.</p>

                    <hr style='margin: 30px 0;' />

                    <p style='font-size: 14px; color: #555;'>
                    <strong>Thông tin liên hệ:</strong><br />
                    📞 Hotline: <a href='tel:+84961075070' style='color: #309689;'>(+84) 961075070</a><br />
                    📧 Email: <a href='mailto:thandea6@gmail.com' style='color: #309689;'>thandea6@gmail.com</a>
                    </p>

                    <p style='margin-top: 20px;'>Trân trọng,<br /><b>Đội ngũ CVMatcher</b></p>";

                    var emailPayload = new { toEmail = User.AccountEmail, subject, message };
                    await _httpClient.PostAsJsonAsync("https://localhost:7004/api/AdminSendRemind/sendRemind", emailPayload);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Cấm user thất bại.";
            }

            return await OnGetAsync(accountId);
        }


        public async Task<IActionResult> OnPostUnbanAsync(int accountId, int userId)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(
                $"https://localhost:7004/api/UserForAdmin/UnbanUser/{userId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã gỡ cấm user thành công.";

                var userResponse = await _httpClient.GetAsync($"https://localhost:7004/api/UserForAdmin/GetUserByAccount/{accountId}");
                if (userResponse.IsSuccessStatusCode)
                {
                    User = await userResponse.Content.ReadFromJsonAsync<UserDetailOfAdminDTO>();

                    string subject = "[CVMatcher] TÀI KHOẢN của bạn đã được KÍCH HOẠT lại";
                    string message = $@"
                    <p>Xin chào <b>{User.FullName ?? "người dùng"}</b>,</p>
                    <p>Tài khoản của bạn trên hệ thống <b>CVMatcher</b> đã được <span style='color:green;'>kích hoạt lại</span> và bạn có thể tiếp tục sử dụng dịch vụ.</p>
                    <p>Chúng tôi rất mong tiếp tục đồng hành cùng bạn trong quá trình tìm kiếm và kết nối cơ hội nghề nghiệp phù hợp.</p>

                    <hr style='margin: 30px 0;' />

                    <p style='font-size: 14px; color: #555;'>
                    <strong>Thông tin liên hệ:</strong><br />
                    📞 Hotline: <a href='tel:+84961075070' style='color: #309689;'>(+84) 961075070</a><br />
                    📧 Email: <a href='mailto:thandea6@gmail.com' style='color: #309689;'>thandea6@gmail.com</a>
                    </p>

                    <p style='margin-top: 20px;'>Trân trọng,<br /><b>Đội ngũ CVMatcher</b></p>";

                    var emailPayload = new { toEmail = User.AccountEmail, subject, message };
                    await _httpClient.PostAsJsonAsync("https://localhost:7004/api/AdminSendRemind/sendRemind", emailPayload);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Gỡ cấm user thất bại.";
            }

            return await OnGetAsync(accountId);
        }





    }
}
