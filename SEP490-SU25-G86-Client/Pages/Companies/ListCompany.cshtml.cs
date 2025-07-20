using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using System.Text.Json.Serialization;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class ListCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<CompanyDTO> Companies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public ListCompanyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task OnGetAsync([FromQuery] int page = 1, int pageSize = 15)
        {
            CurrentPage = page < 1 ? 1 : page;
            Companies = new List<CompanyDTO>();
            TotalPages = 1;

            var url = $"https://localhost:7004/api/Company/list-company?page={CurrentPage}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            var pagedResponse = await response.Content.ReadFromJsonAsync<PagedCompanyResponse>();
            if (pagedResponse != null)
            {
                Companies = pagedResponse.Companies;
                TotalItems = pagedResponse.TotalCount;
                TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
            }
        }

        public class CompanyDTO
        {

            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string LogoUrl { get; set; }
            public string Description { get; set; }
        }
        public class PagedCompanyResponse
        {
            [JsonPropertyName("data")]
            public List<CompanyDTO> Companies { get; set; }

            [JsonPropertyName("totalCount")]
            public int TotalCount { get; set; }
        }
    }
}
