using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminDashboardDTO;

namespace SEP490_SU25_G86_Client.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<JobPostMonthlyStatisticDTO> MonthlyStats { get; set; }


        public List<CompanyToGetDTO> Companies { get; set; }
        public List<CVSubmissionStatisticDTO> CVStats { get; set; }
        public string CurrentMode { get; set; }
        public int SelectedCompanyId { get; set; }

        public AdminDashboardModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public async Task OnGetAsync(int? companyId, string mode = "month")
        {

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            SelectedCompanyId = companyId ?? 0;
            CurrentMode = mode;

            //Lấy danh sách Company
            Companies = await _httpClient.GetFromJsonAsync<List<CompanyToGetDTO>>("https://localhost:7004/api/Admin/Companies");


            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<JobPostMonthlyStatisticDTO>>("https://localhost:7004/api/Admin/Dashboard/JobPostPerMonth");
                MonthlyStats = response ?? new List<JobPostMonthlyStatisticDTO>();
            }
            catch (HttpRequestException ex)
            {
                MonthlyStats = new List<JobPostMonthlyStatisticDTO>();
            }

            //Nếu có chọn Company thì lấy thống kê CVSubmission
            if (SelectedCompanyId > 0)
            {
                CVStats = await _httpClient.GetFromJsonAsync<List<CVSubmissionStatisticDTO>>($"https://localhost:7004/api/Admin/Dashboard/CVSubmissionStats?companyId={SelectedCompanyId}&mode={mode}");
            }
            else
            {
                CVStats = new List<CVSubmissionStatisticDTO>();
            }

        }
    }
} 