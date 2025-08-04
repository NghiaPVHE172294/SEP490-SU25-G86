using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using FluentAssertions;
using System.Net;
using vn.edu.fpt.Controllers; // for LoginResponse DTO

namespace CVMatcher_IntegrationTesting.vn.edu.fpt.Controllers
{
    public class CvSubmissionsControllerIntegrationTests : IClassFixture<WebApplicationFactory<SEP490_SU25_G86_API.Program>>
    {
        private readonly HttpClient _client;

        public CvSubmissionsControllerIntegrationTests(WebApplicationFactory<SEP490_SU25_G86_API.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Helper: Authenticate and get JWT token
        private async Task<string> AuthenticateAndGetToken(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result.Token;
        }

        // Helper: Set JWT token for HttpClient
        private void SetJwtToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Candidate submits their CV to a job post successfully
        /// </summary>
        [Fact(DisplayName = "Candidate submits CV to job post successfully")]
        public async Task SubmitCV_ValidCandidateAndCV_ReturnsOk()
        {
            // Đăng nhập ứng viên (điền email/password test thực tế)
            var candidateEmail = "candidate@email.com"; // <-- điền tài khoản test
            var candidatePassword = "password";
            var token = await AuthenticateAndGetToken(candidateEmail, candidatePassword);
            SetJwtToken(token);

            // Chuẩn bị dữ liệu test
            var submissionDto = new { CvId = 1, JobPostId = 1 }; // <-- điền id hợp lệ
            var response = await _client.PostAsJsonAsync("/api/cvsubmissions", submissionDto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Candidate cannot submit the same CV to the same job post twice
        /// </summary>
        [Fact(DisplayName = "Candidate cannot submit duplicate CV to job post")]
        public async Task SubmitCV_DuplicateSubmission_ReturnsBadRequest()
        {
            var candidateEmail = "candidate@email.com";
            var candidatePassword = "password";
            var token = await AuthenticateAndGetToken(candidateEmail, candidatePassword);
            SetJwtToken(token);

            var submissionDto = new { CvId = 1, JobPostId = 1 }; // <-- điền id hợp lệ
            await _client.PostAsJsonAsync("/api/cvsubmissions", submissionDto);
            var response = await _client.PostAsJsonAsync("/api/cvsubmissions", submissionDto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Candidate cannot submit with invalid CVId or JobPostId
        /// </summary>
        [Fact(DisplayName = "Candidate cannot submit with invalid CVId or JobPostId")]
        public async Task SubmitCV_InvalidIds_ReturnsNotFound()
        {
            var candidateEmail = "candidate@email.com";
            var candidatePassword = "password";
            var token = await AuthenticateAndGetToken(candidateEmail, candidatePassword);
            SetJwtToken(token);

            var submissionDto = new { CvId = -1, JobPostId = -1 };
            var response = await _client.PostAsJsonAsync("/api/cvsubmissions", submissionDto);
            (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest).Should().BeTrue();
        }

        /// <summary>
        /// Employer cannot submit a CV to a job post (permission check)
        /// </summary>
        [Fact(DisplayName = "Employer cannot submit CV to job post (forbidden)")]
        public async Task SubmitCV_Employer_ReturnsForbidden()
        {
            var employerEmail = "employer@email.com"; // <-- điền tài khoản employer test
            var employerPassword = "password";
            var token = await AuthenticateAndGetToken(employerEmail, employerPassword);
            SetJwtToken(token);

            var submissionDto = new { CvId = 1, JobPostId = 1 }; // <-- điền id hợp lệ
            var response = await _client.PostAsJsonAsync("/api/cvsubmissions", submissionDto);
            (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized).Should().BeTrue();
        }

        /// <summary>
        /// Get all CV submissions for a job post (employer only)
        /// </summary>
        [Fact(DisplayName = "Employer gets CV submissions for their job post")]
        public async Task GetCvSubmissions_Employer_ReturnsList()
        {
            var employerEmail = "employer@email.com";
            var employerPassword = "password";
            var token = await AuthenticateAndGetToken(employerEmail, employerPassword);
            SetJwtToken(token);

            var jobPostId = 1; // <-- điền id job post hợp lệ của employer này
            var response = await _client.GetAsync($"/api/cvsubmissions/jobpost/{jobPostId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var submissions = await response.Content.ReadFromJsonAsync<List<CvSubmissionForJobPostDTO>>();
            submissions.Should().NotBeNull();
        }

        /// <summary>
        /// Candidate withdraws their submission
        /// </summary>
        [Fact(DisplayName = "Candidate withdraws their CV submission")]
        public async Task WithdrawSubmission_Candidate_ReturnsOk()
        {
            var candidateEmail = "candidate@email.com";
            var candidatePassword = "password";
            var token = await AuthenticateAndGetToken(candidateEmail, candidatePassword);
            SetJwtToken(token);

            int submissionId = 1; // <-- điền submission id hợp lệ của candidate này
            var response = await _client.PutAsync($"/api/cvsubmissions/withdraw/{submissionId}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
