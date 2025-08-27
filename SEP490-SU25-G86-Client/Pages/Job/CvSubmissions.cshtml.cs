using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class CvSubmissionsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int JobPostId { get; set; }
        public List<CvSubmissionForJobPostDTO> CvSubmissions { get; set; } = new();
        [BindProperty]
        public CVSubRecruiterNoteDTO RecruiterNoteForm { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? JobPostTitle { get; set; }
        public string? CompanyName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? CandidateNameFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public double? ScoreMin { get; set; }
        [BindProperty(SupportsGet = true)]
        public double? ScoreMax { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? SubmissionDateFrom { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? SubmissionDateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public async Task OnGetAsync()
        {
            if (JobPostId <= 0)
            {
                ErrorMessage = "Thiếu JobPostId.";
                return;
            }
            try
            {
                var client = new HttpClient();
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Fetch job post details
                var jobPostResponse = await client.GetAsync($"https://localhost:7004/api/JobPosts/{JobPostId}");
                if (jobPostResponse.IsSuccessStatusCode)
                {
                    var jobPostJson = await jobPostResponse.Content.ReadAsStringAsync();
                    var jobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(jobPostJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    JobPostTitle = jobPostDetail?.Title;
                    CompanyName = jobPostDetail?.CompanyName;
                }
                // Fetch CV submissions
                var apiUrl = $"https://localhost:7004/api/cvsubmissions/jobpost/{JobPostId}";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    CvSubmissions = JsonSerializer.Deserialize<List<CvSubmissionForJobPostDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    // Lọc theo filter
                    if (!string.IsNullOrEmpty(StatusFilter))
                        CvSubmissions = CvSubmissions.Where(x => x.Status != null && x.Status.Equals(StatusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (!string.IsNullOrEmpty(CandidateNameFilter))
                        CvSubmissions = CvSubmissions.Where(x => x.CandidateName != null && x.CandidateName.Contains(CandidateNameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (ScoreMin.HasValue)
                        CvSubmissions = CvSubmissions.Where(x => x.TotalScore.HasValue && x.TotalScore.Value >= ScoreMin.Value).ToList();
                    if (ScoreMax.HasValue)
                        CvSubmissions = CvSubmissions.Where(x => x.TotalScore.HasValue && x.TotalScore.Value <= ScoreMax.Value).ToList();
                    if (SubmissionDateFrom.HasValue)
                        CvSubmissions = CvSubmissions.Where(x => x.SubmissionDate.HasValue && x.SubmissionDate.Value.Date >= SubmissionDateFrom.Value.Date).ToList();
                    if (SubmissionDateTo.HasValue)
                        CvSubmissions = CvSubmissions.Where(x => x.SubmissionDate.HasValue && x.SubmissionDate.Value.Date <= SubmissionDateTo.Value.Date).ToList();

                    // Phân trang
                    TotalRecords = CvSubmissions.Count;
                    TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
                    if (PageIndex < 1) PageIndex = 1;
                    if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;
                    CvSubmissions = CvSubmissions.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                else
                {
                    ErrorMessage = $"Không thể tải danh sách CV: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent("\"ĐÃ DUYỆT\"", System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7004/api/cvsubmissions/{id}/status", content);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Không thể duyệt CV.";
                return RedirectToPage(new { JobPostId });
            }

            try
            {
                var listResp = await client.GetAsync($"https://localhost:7004/api/cvsubmissions/jobpost/{JobPostId}");
                if (listResp.IsSuccessStatusCode)
                {
                    var json = await listResp.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<CvSubmissionForJobPostDTO>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                    var submission = list.FirstOrDefault(x => x.SubmissionId == id);

                    // lấy title để chèn vào content (nếu cần)
                    var jobTitle = await GetJobTitleAsync();

                    if (submission?.CandidateId != null)
                    {
                        // ✅ generate absolute URL đúng route Razor Page
                        var jobUrl = Url.Page(
                            pageName: "/Job/DetailJobPost",
                            pageHandler: null,
                            values: new { id = JobPostId },
                            protocol: Request.Scheme
                        );

                        var notifyPayload = new
                        {
                            ReceiverUserId = (long)submission.CandidateId.Value,
                            Content = $"Chúc mừng {submission.CandidateName}! CV cho tin '{jobTitle ?? "tuyển dụng"}' đã được DUYỆT.",
                            TargetUrl = jobUrl
                        };

                        var notifyJson = JsonSerializer.Serialize(notifyPayload,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        var notifyContent = new StringContent(notifyJson, System.Text.Encoding.UTF8, "application/json");

                        var notifyResp = await client.PostAsync("https://localhost:7004/api/notifications", notifyContent);
                        if (!notifyResp.IsSuccessStatusCode)
                        {
                            var body = await notifyResp.Content.ReadAsStringAsync();
                            Console.WriteLine($"[Notify] Approve send failed {notifyResp.StatusCode}: {body}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Notify][Approve] " + ex);
            }

            TempData["SuccessMessage"] = "Duyệt CV thành công!";
            return RedirectToPage(new { JobPostId });
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent("\"ĐÃ TỪ CHỐI\"", System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7004/api/cvsubmissions/{id}/status", content);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Không thể từ chối CV.";
                return RedirectToPage(new { JobPostId });
            }

            try
            {
                var listResp = await client.GetAsync($"https://localhost:7004/api/cvsubmissions/jobpost/{JobPostId}");
                if (listResp.IsSuccessStatusCode)
                {
                    var json = await listResp.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<CvSubmissionForJobPostDTO>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                    var submission = list.FirstOrDefault(x => x.SubmissionId == id);

                    var jobTitle = await GetJobTitleAsync();

                    if (submission?.CandidateId != null)
                    {
                        // ✅ absolute URL đúng trang chi tiết
                        var jobUrl = Url.Page("/Job/DetailJobPost", null, new { id = JobPostId }, Request.Scheme);

                        var notifyPayload = new
                        {
                            ReceiverUserId = (long)submission.CandidateId.Value,
                            Content = $"Rất tiếc {submission.CandidateName}, CV cho tin '{jobTitle ?? "tuyển dụng"}' chưa phù hợp ở thời điểm hiện tại.",
                            TargetUrl = jobUrl
                        };

                        var notifyJson = JsonSerializer.Serialize(notifyPayload,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        var notifyContent = new StringContent(notifyJson, System.Text.Encoding.UTF8, "application/json");

                        var notifyResp = await client.PostAsync("https://localhost:7004/api/notifications", notifyContent);
                        if (!notifyResp.IsSuccessStatusCode)
                        {
                            var body = await notifyResp.Content.ReadAsStringAsync();
                            Console.WriteLine($"[Notify] Reject send failed {notifyResp.StatusCode}: {body}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Notify][Reject] " + ex);
            }

            TempData["SuccessMessage"] = "Từ chối CV thành công!";
            return RedirectToPage(new { JobPostId });
        }

        public async Task<IActionResult> OnPostAIFilterAsync(int id)
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Luôn fetch lại submissions từ API để đảm bảo dữ liệu mới nhất
            var apiUrl = $"https://localhost:7004/api/cvsubmissions/jobpost/{JobPostId}";
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CvSubmissions = System.Text.Json.JsonSerializer.Deserialize<List<CvSubmissionForJobPostDTO>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                ErrorMessage = $"Không thể tải danh sách CV: {response.ReasonPhrase}";
                return RedirectToPage(new { JobPostId });
            }

            // Lấy submission để lấy hai id cần thiết
            var submission = CvSubmissions.FirstOrDefault(x => x.SubmissionId == id);
            if (submission == null || submission.CvParsedDataId == null || submission.JobCriteriaId == null)
            {
                ErrorMessage = "Không đủ dữ liệu để lọc AI cho CV này!";
                return RedirectToPage(new { JobPostId });
            }
            var body = new
            {
                cvParsedDataId = submission.CvParsedDataId,
                jobCriteriaId = submission.JobCriteriaId
            };
            var jsonBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

            var aiResponse = await client.PostAsync(
                $"https://localhost:7004/api/AI/CompareCvWithJobCriteria", jsonBody);

            if (aiResponse.IsSuccessStatusCode)
            {
                // Reload lại submissions để cập nhật điểm và trạng thái mới
                await OnGetAsync();
                TempData["SuccessMessage"] = "Lọc AI thành công!";
            }
            else
            {
                ErrorMessage = "Không thể lọc AI cho CV này!";
            }

            return RedirectToPage(new { JobPostId });
        }
        public async Task<IActionResult> OnPostUpdateNoteAsync()
        {
            Console.WriteLine($"SUBMISSION ID = {RecruiterNoteForm.SubmissionId}, NOTE = {RecruiterNoteForm.RecruiterNote}");
            try
            {
                var client = new HttpClient();
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(RecruiterNoteForm, options);
                Console.WriteLine("JSON gửi lên: " + json);  // Debug JSON body
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");


                var response = await client.PatchAsync("https://localhost:7004/api/CvSubmissions/recruiter-note", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API response: " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật ghi chú thành công!";
                }
                else
                {
                    ErrorMessage = "Không thể cập nhật ghi chú.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi ghi chú: {ex.Message}";
            }

            return RedirectToPage(new { JobPostId });
        }
        private async Task<string?> GetJobTitleAsync()
        {
            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.GetAsync($"https://localhost:7004/api/JobPosts/{JobPostId}");
            if (!resp.IsSuccessStatusCode) return null;

            var jobPostJson = await resp.Content.ReadAsStringAsync();
            var jobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(
                jobPostJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return jobPostDetail?.Title;
        }

        private string GetAppBaseUrl()
        {
            // => https://yourdomain.com
            return $"{Request.Scheme}://{Request.Host}";
        }

    }
}