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
    }
    public class CVTemplatesByPositionModel : PageModel
    {
        public List<CVTemplatePublicDTO> Templates { get; set; } = new();
        public List<IndustryDTO> Industries { get; set; } = new();
        public List<JobPositionDTO> Positions { get; set; } = new();

        public int TotalCount { get; set; }
public int Page { get; set; } = 1;
public int PageSize { get; set; } = 3;

public async Task OnGetAsync()
{
    var client = new HttpClient();
    // Lấy danh sách ngành
    var industryRes = await client.GetAsync("https://localhost:7004/api/industries");
    if (industryRes.IsSuccessStatusCode)
    {
        var json = await industryRes.Content.ReadAsStringAsync();
        Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }
    // Lấy danh sách vị trí
    var posRes = await client.GetAsync("https://localhost:7004/api/jobpositions");
    if (posRes.IsSuccessStatusCode)
    {
        var json = await posRes.Content.ReadAsStringAsync();
        Positions = JsonSerializer.Deserialize<List<JobPositionDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }
    // Xử lý filter và phân trang từ query string
    var industryId = Request.Query["industryId"].ToString();
var positionId = Request.Query["positionId"].ToString();
var search = Request.Query["search"].ToString();
var pageStr = Request.Query["page"].ToString();
    int page = 1;
    int.TryParse(pageStr, out page);
    if (page < 1) page = 1;
    Page = page;
    var url = $"https://localhost:7004/api/public/cv-templates?page={page}&pageSize={PageSize}";
    if (!string.IsNullOrEmpty(industryId)) url += $"&industryId={industryId}";
    if (!string.IsNullOrEmpty(positionId)) url += $"&positionId={positionId}";
    if (!string.IsNullOrEmpty(search)) url += $"&search={System.Net.WebUtility.UrlEncode(search)}";
    var response = await client.GetAsync(url);
    if (response.IsSuccessStatusCode)
    {
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CVTemplateApiResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Templates = result?.data ?? new();
        TotalCount = result?.totalCount ?? 0;
    }
}
        public class CVTemplateApiResult
        {
            public List<CVTemplatePublicDTO> data { get; set; }
            public int totalCount { get; set; }
        }
    }
}
