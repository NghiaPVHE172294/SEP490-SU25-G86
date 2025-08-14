using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http; // for Session
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class AdminCVTemplatesModel : PageModel
{
    public List<CvTemplateVM> Templates { get; set; } = new();
    public List<IndustryVM> Industries { get; set; } = new();
    public List<PositionVM> Positions { get; set; } = new();

    [BindProperty]
    public UploadCvTemplateInput UploadInput { get; set; }

    public async Task OnGetAsync()
    {
        var baseUrl = "https://localhost:7004";
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        var token = HttpContext.Session.GetString("jwt_token");
        try
        {
            // Only add Authorization for endpoints that require it
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Templates = await client.GetFromJsonAsync<List<CvTemplateVM>>("/api/admin/cv-templates");

            // Remove Authorization header for public endpoints
            client.DefaultRequestHeaders.Authorization = null;
            Industries = await client.GetFromJsonAsync<List<IndustryVM>>("/api/industry");
            Positions = await client.GetFromJsonAsync<List<PositionVM>>("/api/jobposition");
        }
        catch (HttpRequestException ex)
        {
            // Handle 401/403/other errors gracefully
            ModelState.AddModelError(string.Empty, $"Không thể tải dữ liệu từ API: {ex.Message}");
            Templates = new List<CvTemplateVM>();
            Industries = new List<IndustryVM>();
            Positions = new List<PositionVM>();
        }
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        // Xử lý upload template mới
        var baseUrl = "https://localhost:7004";
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        var token = HttpContext.Session.GetString("jwt_token");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(UploadInput.CvTemplateName ?? ""), "CvTemplateName");
        form.Add(new StringContent(UploadInput.IndustryId.ToString()), "IndustryId");
        form.Add(new StringContent(UploadInput.PositionId.ToString()), "PositionId");
        form.Add(new StringContent(UploadInput.Notes ?? ""), "Notes");
        form.Add(new StreamContent(UploadInput.PdfFile.OpenReadStream()), "PdfFile", UploadInput.PdfFile.FileName);
        form.Add(new StreamContent(UploadInput.DocFile.OpenReadStream()), "DocFile", UploadInput.DocFile.FileName);
        form.Add(new StreamContent(UploadInput.PreviewImage.OpenReadStream()), "PreviewImage", UploadInput.PreviewImage.FileName);
        var resp = await client.PostAsync("/api/admin/cv-templates/upload", form);
        if (resp.IsSuccessStatusCode)
            return RedirectToPage();
        // Xử lý lỗi nếu cần
        ModelState.AddModelError(string.Empty, "Tải lên thất bại");
        await OnGetAsync();
        return Page();
    }

    public class CvTemplateVM
    {
        public int CvTemplateId { get; set; }
        public string CvTemplateName { get; set; }
        public string PdfFileUrl { get; set; }
        public string DocFileUrl { get; set; }
        public string ImgTemplate { get; set; }
        public string Notes { get; set; }
        public string UploadDate { get; set; }
        public int IndustryId { get; set; }
        public int PositionId { get; set; }
    }
    public class IndustryVM { public int IndustryId { get; set; } public string IndustryName { get; set; } }
    public class PositionVM { public int PositionId { get; set; } public string PositionName { get; set; } }
    public class UploadCvTemplateInput
    {
        [Required] public string CvTemplateName { get; set; }
        [Required] public int IndustryId { get; set; }
        [Required] public int PositionId { get; set; }
        public string Notes { get; set; }
        [Required] public Microsoft.AspNetCore.Http.IFormFile PdfFile { get; set; }
        [Required] public Microsoft.AspNetCore.Http.IFormFile DocFile { get; set; }
        [Required] public Microsoft.AspNetCore.Http.IFormFile PreviewImage { get; set; }
    }
}
