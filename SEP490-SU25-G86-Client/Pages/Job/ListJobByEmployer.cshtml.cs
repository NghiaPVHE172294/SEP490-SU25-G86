using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
// Th�m namespace c?a DTO t? API
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class ListJobByEmployerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<JobPostDTO> Jobs { get; set; }

        public ListJobByEmployerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int employerId))
            {
                Jobs = new List<JobPostDTO>();
                return RedirectToPage("/Common/Login");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/JobPosts/employer/{employerId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Jobs = JsonSerializer.Deserialize<List<JobPostDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                Jobs = new List<JobPostDTO>();
            }

            return Page();
        }
    }
}