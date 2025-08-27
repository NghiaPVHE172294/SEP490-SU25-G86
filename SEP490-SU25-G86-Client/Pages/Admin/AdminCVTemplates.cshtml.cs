using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SEP490_SU25_G86_Client.Pages.Admin
{
    public class AdminCVTemplateDTO
{
    public int CvTemplateId { get; set; }
    public string? CvTemplateName { get; set; }
    public string? PdfFileUrl { get; set; }
    public string? DocFileUrl { get; set; }
    public string? ImgTemplate { get; set; }
    public string? UploadDate { get; set; } // Đổi sang string để tránh lỗi JSON
    public string? Notes { get; set; }
    public int IndustryId { get; set; }
    public int PositionId { get; set; }
    public string? IndustryName { get; set; }
    public string? PositionName { get; set; }

    public DateTime? UploadDateParsed
    {
        get
        {
            if (DateTime.TryParse(UploadDate, out var dt))
                return dt;
            return null;
        }
    }
}

    public class IndustryDTO
    {
        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = string.Empty;
    }
    public class PositionDTO
    {
        public int PositionId { get; set; }
        public string PostitionName { get; set; } = string.Empty;
    }
    public class AdminCVTemplatesModel : PageModel
    {
        public List<AdminCVTemplateDTO> Templates { get; set; } = new();
        public List<IndustryDTO> Industries { get; set; } = new();
        public List<PositionDTO> Positions { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;

        [BindProperty]
        public string? CvTemplateName { get; set; }
        [BindProperty]
        public string? Notes { get; set; }
        [BindProperty]
        public IFormFile? PdfFile { get; set; }
        [BindProperty]
        public IFormFile? DocFile { get; set; }
        [BindProperty]
        public IFormFile? PreviewImage { get; set; }
        [BindProperty]
        public int TemplateId { get; set; }
        [BindProperty]
        public int IndustryId { get; set; }
        [BindProperty]
        public int PositionId { get; set; }

        private const string ApiBase = "https://localhost:7004/";

        public async Task OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "ADMIN")
            {
                Response.Redirect("/NotFound");
                return;
            }
            var token = HttpContext.Session.GetString("jwt_token");
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Đọc query string
            var searchName = Request.Query["SearchName"].ToString()?.Trim();
            var dateFromStr = Request.Query["DateFrom"].ToString();
            var dateToStr = Request.Query["DateTo"].ToString();
            int page = 1;
            int.TryParse(Request.Query["page"], out page);
            if (page < 1) page = 1;
            PageIndex = page;
            PageSize = 10;

            // Load industries
            var resIndustry = await client.GetAsync(ApiBase + "api/industries");
    if (resIndustry.IsSuccessStatusCode)
    {
        var json = await resIndustry.Content.ReadAsStringAsync();
        Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }
    // Load positions
    var resPosition = await client.GetAsync(ApiBase + "api/jobpositions");
    if (resPosition.IsSuccessStatusCode)
    {
        var json = await resPosition.Content.ReadAsStringAsync();
        Positions = JsonSerializer.Deserialize<List<PositionDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    // Load all templates
    var response = await client.GetAsync(ApiBase + "api/admin/cv-templates");
    var allTemplates = new List<AdminCVTemplateDTO>();
    if (response.IsSuccessStatusCode)
    {
        var json = await response.Content.ReadAsStringAsync();
        allTemplates = JsonSerializer.Deserialize<List<AdminCVTemplateDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        // Lọc dữ liệu
        if (!string.IsNullOrEmpty(searchName))
            allTemplates = allTemplates.Where(t => t.CvTemplateName != null && t.CvTemplateName.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (DateTime.TryParse(dateFromStr, out var dateFrom))
            allTemplates = allTemplates.Where(t => t.UploadDateParsed != null && t.UploadDateParsed.Value.Date >= dateFrom.Date).ToList();
        if (DateTime.TryParse(dateToStr, out var dateTo))
            allTemplates = allTemplates.Where(t => t.UploadDateParsed != null && t.UploadDateParsed.Value.Date <= dateTo.Date).ToList();

        // Phân trang
        TotalCount = allTemplates.Count;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        if (PageIndex > TotalPages) PageIndex = TotalPages > 0 ? TotalPages : 1;
        Templates = allTemplates.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

        // Map IndustryName, PositionName
        foreach (var t in Templates)
        {
            t.IndustryName = Industries.FirstOrDefault(i => i.IndustryId == t.IndustryId)?.IndustryName ?? "";
            t.PositionName = Positions.FirstOrDefault(p => p.PositionId == t.PositionId)?.PostitionName ?? "";
        }
    }
    else
    {
        Templates = new List<AdminCVTemplateDTO>();
        TotalPages = 1;
    }
}
        // Handler upload template
        [TempData]
        public string? UploadError { get; set; }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            string debugLog = "==== DEBUG UPLOAD ====";
            debugLog += "\nToken: " + (token ?? "NULL");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                debugLog += "\nAuthorization Header: Bearer ...";
            }
            else
            {
                debugLog += "\nKhông tìm thấy JWT token trong session!";
            }
            debugLog += $"\nEndpoint: {ApiBase}api/admin/cv-templates/upload";

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(CvTemplateName ?? ""), "CvTemplateName");
            form.Add(new StringContent(Notes ?? ""), "Notes");
            form.Add(new StringContent(IndustryId.ToString()), "IndustryId");
            form.Add(new StringContent(PositionId.ToString()), "PositionId");
            if (PdfFile != null)
            {
                var pdfContent = new StreamContent(PdfFile.OpenReadStream());
                pdfContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                form.Add(pdfContent, "PdfFile", PdfFile.FileName);
            }
            if (DocFile != null)
            {
                var docContent = new StreamContent(DocFile.OpenReadStream());
                // Xác định ContentType phù hợp cho .doc hoặc .docx
                var ext = System.IO.Path.GetExtension(DocFile.FileName).ToLower();
                if (ext == ".doc")
                    docContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/msword");
                else
                    docContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                form.Add(docContent, "DocFile", DocFile.FileName);
            }
// Nếu DocFile == null thì KHÔNG add field DocFile vào form, đảm bảo backend không nhận field rỗng.
            if (PreviewImage != null)
            {
                var imgContent = new StreamContent(PreviewImage.OpenReadStream());
                imgContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(PreviewImage.ContentType);
                form.Add(imgContent, "PreviewImage", PreviewImage.FileName);
            }

            // Log các trường gửi lên
            debugLog += $"\nCvTemplateName: {CvTemplateName}, Notes: {Notes}, IndustryId: {IndustryId}, PositionId: {PositionId}";
            debugLog += $"\nPdfFile: {(PdfFile?.FileName ?? "NULL")}, DocFile: {(DocFile?.FileName ?? "NULL")}, PreviewImage: {(PreviewImage?.FileName ?? "NULL")}";

            var response = await client.PostAsync(ApiBase + "api/admin/cv-templates/upload", form);
            debugLog += $"\nResponse Status: {response.StatusCode}";
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                debugLog += $"\nError: {error}";
                UploadError = $"Upload thất bại: {error}<br><pre>{debugLog}</pre>";
                return RedirectToPage();
            }
            // KHÔNG set UploadError khi thành công
            return RedirectToPage();
        }

        // Handler edit template
        public async Task<IActionResult> OnPostEditAsync()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var data = new
            {
                CvTemplateName = CvTemplateName,
                Notes = Notes
            };
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(ApiBase + $"api/admin/cv-templates/{TemplateId}", content);
            // Có thể xử lý lỗi nếu cần
            return RedirectToPage();
        }

        // Handler delete template
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync(ApiBase + $"api/admin/cv-templates/{TemplateId}");
            // Có thể xử lý lỗi nếu cần
            return RedirectToPage();
        }
    }
}
