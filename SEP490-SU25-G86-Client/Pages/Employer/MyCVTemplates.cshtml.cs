using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class CvTemplateDTO
    {
        public int CvtemplateOfEmployerId { get; set; }
        public string? CvTemplateName { get; set; }
        public string? PdfFileUrl { get; set; }
        public string? DocFileUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? Notes { get; set; }
    }

    public class MyCVTemplatesModel : PageModel
    {
        public List<CvTemplateDTO> Templates { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;

        [BindProperty]
        public string? CvTemplateName { get; set; }
        [BindProperty]
        public string? Notes { get; set; }
        [BindProperty]
        public IFormFile? PdfFile { get; set; }
        [BindProperty]
        public IFormFile? DocFile { get; set; }
        [BindProperty]
        public int TemplateId { get; set; }

        private const string ApiBase = "https://localhost:7004/";

        public async Task OnGetAsync()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(ApiBase + "api/employer/cv-templates");
            var allTemplates = new List<CvTemplateDTO>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                allTemplates = JsonSerializer.Deserialize<List<CvTemplateDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            // FILTER
            var searchName = Request.Query["SearchName"].ToString()?.Trim();
            var dateFromStr = Request.Query["DateFrom"].ToString();
            var dateToStr = Request.Query["DateTo"].ToString();
            DateTime? dateFrom = null, dateTo = null;
            if (DateTime.TryParse(dateFromStr, out var dtFrom)) dateFrom = dtFrom.Date;
            if (DateTime.TryParse(dateToStr, out var dtTo)) dateTo = dtTo.Date.AddDays(1).AddTicks(-1);

            var filtered = allTemplates.AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
                filtered = filtered.Where(t => (t.CvTemplateName ?? "").ToLower().Contains(searchName.ToLower()));
            if (dateFrom.HasValue)
                filtered = filtered.Where(t => t.UploadDate >= dateFrom);
            if (dateTo.HasValue)
                filtered = filtered.Where(t => t.UploadDate <= dateTo);

            // PAGINATION
            int pageIndex = 1, pageSize = 10;
            int.TryParse(Request.Query["PageIndex"], out pageIndex);
            int.TryParse(Request.Query["PageSize"], out pageSize);
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            int totalRecords = filtered.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            PageIndex = pageIndex;
            PageSize = pageSize;
            Templates = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (PdfFile == null || DocFile == null || string.IsNullOrEmpty(CvTemplateName))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn đủ file PDF, DOC/DOCX và nhập tên template.");
                await OnGetAsync();
                return Page();
            }

            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(PdfFile.OpenReadStream()), "PdfFile", PdfFile.FileName);
            content.Add(new StreamContent(DocFile.OpenReadStream()), "DocFile", DocFile.FileName);
            content.Add(new StringContent(CvTemplateName), "CvTemplateName");
            if (!string.IsNullOrEmpty(Notes))
                content.Add(new StringContent(Notes), "Notes");

            var response = await client.PostAsync(ApiBase + "api/employer/cv-templates/upload", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Tải lên thất bại: " + errorMsg);
                await OnGetAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Tải lên CV template thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (TemplateId == 0 || string.IsNullOrEmpty(CvTemplateName))
            {
                ModelState.AddModelError(string.Empty, "Thiếu thông tin cập nhật.");
                await OnGetAsync();
                return Page();
            }

            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateData = new
            {
                CvTemplateName = CvTemplateName,
                Notes = Notes
            };

            var json = JsonSerializer.Serialize(updateData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(ApiBase + $"api/employer/cv-templates/{TemplateId}", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Cập nhật thất bại: " + errorMsg);
                await OnGetAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Cập nhật template thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (TemplateId == 0)
            {
                ModelState.AddModelError(string.Empty, "Thiếu thông tin xóa.");
                await OnGetAsync();
                return Page();
            }

            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync(ApiBase + $"api/employer/cv-templates/{TemplateId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Xóa thất bại: " + errorMsg);
                await OnGetAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Xóa template thành công!";
            return RedirectToPage();
        }
    }
}
