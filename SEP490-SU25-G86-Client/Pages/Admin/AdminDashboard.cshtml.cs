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

            // Lấy danh sách Company
            Companies = await _httpClient.GetFromJsonAsync<List<CompanyToGetDTO>>("https://localhost:7004/api/Admin/Companies");


            //try
            //{
            //    var response = await _httpClient.GetFromJsonAsync<List<JobPostMonthlyStatisticDTO>>("https://localhost:7004/api/Admin/Dashboard/JobPostPerMonth");
            //    MonthlyStats = response ?? new List<JobPostMonthlyStatisticDTO>();
            //}
            //catch (HttpRequestException ex)
            //{
            //    MonthlyStats = new List<JobPostMonthlyStatisticDTO>();
            //}

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

            // Nếu có chọn Company thì lấy thống kê CVSubmission
            //if (SelectedCompanyId > 0)
            //{
            //    CVStats = await _httpClient.GetFromJsonAsync<List<CVSubmissionStatisticDTO>>($"https://localhost:7004/api/Admin/Dashboard/CVSubmissionStats?companyId={SelectedCompanyId}&mode={mode}");
            //}
            //else
            //{
            //    CVStats = new List<CVSubmissionStatisticDTO>();
            //}


            //Fake data for CVStats
            if (SelectedCompanyId > 0)
            {
                if (mode == "year")
                {
                    switch (SelectedCompanyId)
                    {
                        case 1: // FPT Software
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2022, Count = 120 },
                    new CVSubmissionStatisticDTO { Year = 2023, Count = 145 },
                    new CVSubmissionStatisticDTO { Year = 2024, Count = 160 },
                    new CVSubmissionStatisticDTO { Year = 2025, Count = 180 }
                };
                            break;

                        case 2: // Vietcombank
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2022, Count = 50 },
                    new CVSubmissionStatisticDTO { Year = 2023, Count = 65 },
                    new CVSubmissionStatisticDTO { Year = 2024, Count = 70 },
                    new CVSubmissionStatisticDTO { Year = 2025, Count = 80 }
                };
                            break;

                        case 3: // VNPT Education
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2022, Count = 200 },
                    new CVSubmissionStatisticDTO { Year = 2023, Count = 230 },
                    new CVSubmissionStatisticDTO { Year = 2024, Count = 250 },
                    new CVSubmissionStatisticDTO { Year = 2025, Count = 300 }
                };
                            break;

                        case 4: // Vinmec
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2022, Count = 70 },
                    new CVSubmissionStatisticDTO { Year = 2023, Count = 85 },
                    new CVSubmissionStatisticDTO { Year = 2024, Count = 95 },
                    new CVSubmissionStatisticDTO { Year = 2025, Count = 110 }
                };
                            break;

                        default:
                            CVStats = new List<CVSubmissionStatisticDTO>();
                            break;
                    }
                }
                else // mode == month
                {
                    switch (SelectedCompanyId)
                    {
                        case 1:
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 1, Count = 15 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 2, Count = 25 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 3, Count = 20 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 4, Count = 35 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 5, Count = 40 }
                };
                            break;

                        case 2:
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 1, Count = 5 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 2, Count = 12 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 3, Count = 18 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 4, Count = 22 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 5, Count = 27 }
                };
                            break;

                        case 3:
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 1, Count = 30 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 2, Count = 35 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 3, Count = 32 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 4, Count = 40 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 5, Count = 50 }
                };
                            break;

                        case 4:
                            CVStats = new List<CVSubmissionStatisticDTO>
                {
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 1, Count = 8 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 2, Count = 15 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 3, Count = 12 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 4, Count = 18 },
                    new CVSubmissionStatisticDTO { Year = 2025, Month = 5, Count = 22 }
                };
                            break;

                        default:
                            CVStats = new List<CVSubmissionStatisticDTO>();
                            break;
                    }
                }
            }
            else
            {
                CVStats = new List<CVSubmissionStatisticDTO>();
            }
        }
    }
} 