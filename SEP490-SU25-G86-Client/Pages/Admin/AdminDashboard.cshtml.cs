using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminDashboardDTO;

namespace SEP490_SU25_G86_Client.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<JobPostMonthlyStatisticDTO> MonthlyStats { get; set; }

        public AdminDashboardModel(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        public async Task OnGetAsync()
        {
            //var response = await _httpClient.GetFromJsonAsync<List<JobPostMonthlyStatisticDTO>>("https://localhost:7004/api/Admin/statistics/monthly-jobposts");
            //MonthlyStats = response ?? new List<JobPostMonthlyStatisticDTO>();


            MonthlyStats = new List<JobPostMonthlyStatisticDTO>
            {
                new JobPostMonthlyStatisticDTO { Month = 1, Year = 2025, Count = 10 },
                new JobPostMonthlyStatisticDTO { Month = 2, Year = 2025, Count = 20 },
                new JobPostMonthlyStatisticDTO { Month = 3, Year = 2025, Count = 30 },
                new JobPostMonthlyStatisticDTO { Month = 4, Year = 2025, Count = 25 },
                new JobPostMonthlyStatisticDTO { Month = 5, Year = 2025, Count = 35 },
                new JobPostMonthlyStatisticDTO { Month = 6, Year = 2025, Count = 40 },
                new JobPostMonthlyStatisticDTO { Month = 7, Year = 2025, Count = 38 },
                new JobPostMonthlyStatisticDTO { Month = 8, Year = 2025, Count = 42 }
            };
        }
    }
} 