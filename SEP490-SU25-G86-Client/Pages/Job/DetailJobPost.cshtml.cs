using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class DetailJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public ViewDetailJobPostDTO? JobPostDetail { get; set; }
        public string? ErrorMessage { get; set; }

        public DetailJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/JobPosts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Không tìm thấy tin tuyển dụng.";
                }
                else
                {
                    ErrorMessage = $"Lỗi: {response.StatusCode}";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }
            return Page();
        }
    }
} 