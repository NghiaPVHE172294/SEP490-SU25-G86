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

    }
}
