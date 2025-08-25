using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;

namespace SEP490_SU25_G86_Client.Pages
{
    public class CVTemplatePublicDTO
    {
        public int CvTemplateId { get; set; }
        public string CvTemplateName { get; set; }
        public string PdfFileUrl { get; set; }
        public string DocFileUrl { get; set; }
        public string ImgTemplate { get; set; }
        public string PositionName { get; set; }
    }
    public class IndustryDTO
    {
        public int IndustryId { get; set; }
        public string IndustryName { get; set; }
    }
    public class JobPositionDTO
    {
        public int PositionId { get; set; }
        public string PostitionName { get; set; }
        public int IndustryId { get; set; }
    }
    public class CVTemplatesByPositionModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CVTemplatesByPositionModel> _logger;

        public CVTemplatesByPositionModel(IHttpClientFactory httpClientFactory, ILogger<CVTemplatesByPositionModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public List<CVTemplatePublicDTO> Templates { get; set; } = new();
        public List<IndustryDTO> Industries { get; set; } = new();
        public List<JobPositionDTO> Positions { get; set; } = new();

        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int? SelectedIndustryId { get; set; }
        public int? SelectedPositionId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                
                // Lấy danh sách ngành
                await LoadIndustriesAsync(client);
                
                // Xử lý query parameters
                Page = int.TryParse(Request.Query["page"].ToString(), out var page) ? page : 1;
                SelectedIndustryId = int.TryParse(Request.Query["industryId"].ToString(), out var industryId) ? industryId : (int?)null;
                SelectedPositionId = int.TryParse(Request.Query["positionId"].ToString(), out var positionId) ? positionId : (int?)null;
                SearchTerm = Request.Query["search"].ToString() ?? string.Empty;

                // Lấy danh sách vị trí dựa trên ngành đã chọn (nếu có)
                await LoadPositionsAsync(client, SelectedIndustryId);
                
                // Lấy danh sách mẫu CV với bộ lọc
                await LoadTemplatesAsync(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CV templates by position");
                // Có thể thêm thông báo lỗi cho người dùng nếu cần
            }
        }

        private async Task LoadIndustriesAsync(HttpClient client)
        {
            var response = await client.GetAsync("https://localhost:7004/api/industries");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        private async Task LoadPositionsAsync(HttpClient client, int? industryId)
        {
            var url = industryId.HasValue 
                ? $"https://localhost:7004/api/JobPositions/by-industry/{industryId}"
                : "https://localhost:7004/api/JobPositions";
                
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Positions = JsonSerializer.Deserialize<List<JobPositionDTO>>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        private async Task LoadTemplatesAsync(HttpClient client)
        {
            try
            {
                var queryParams = new List<string>();
                if (SelectedIndustryId.HasValue)
                    queryParams.Add($"industryId={SelectedIndustryId}");
                
                if (SelectedPositionId.HasValue)
                    queryParams.Add($"positionId={SelectedPositionId}");
                
                if (!string.IsNullOrEmpty(SearchTerm))
                    queryParams.Add($"search={Uri.EscapeDataString(SearchTerm)}");
                
                queryParams.Add($"page={Page}");
                queryParams.Add($"pageSize={PageSize}");
            
                var queryString = string.Join("&", queryParams);
                var url = $"https://localhost:7004/api/public/cv-templates?{queryString}";
            
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<CVTemplateApiResult>(
                        json, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                
                    if (result != null)
                    {
                        Templates = result.data ?? new();
                        TotalCount = result.totalCount;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CV templates");
                Templates = new();
                TotalCount = 0;
            }
        }

        public class CVTemplateApiResult
        {
            public List<CVTemplatePublicDTO> data { get; set; } = new();
            public int totalCount { get; set; }
        }
    }
}
