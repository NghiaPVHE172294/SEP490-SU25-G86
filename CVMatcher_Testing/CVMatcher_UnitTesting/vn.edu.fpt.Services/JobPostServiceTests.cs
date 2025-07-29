using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using Xunit;

namespace CVMatcher_Testing.vn.edu.fpt.Services
{
    public class JobPostServiceTests
    {
        private readonly Mock<IJobPostRepository> _jobPostRepoMock;
        private readonly Mock<IBlockedCompanyRepository> _blockedCompanyRepoMock;
        private readonly JobPostService _service;

        public JobPostServiceTests()
        {
            _jobPostRepoMock = new Mock<IJobPostRepository>();
            _blockedCompanyRepoMock = new Mock<IBlockedCompanyRepository>();
            _service = new JobPostService(_jobPostRepoMock.Object, _blockedCompanyRepoMock.Object);
        }

        [Fact]
        public async Task LayTatCaJobPostTraVeDung()
        {
            // Sắp xếp
            var posts = new List<JobPost>
            {
                new JobPost { JobPostId = 1, Title = "Job 1" },
                new JobPost { JobPostId = 2, Title = "Job 2" }
            };
            _jobPostRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(posts);

            // Hành động
            var result = await _service.GetAllJobPostsAsync();

            // Kiểm tra
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.JobPostId == 1);
            Assert.Contains(result, p => p.JobPostId == 2);
        }

        [Fact]
        public async Task LayJobPostTheoEmployerIdTraVeDung()
        {
            // Sắp xếp
            int employerId = 5;
            var posts = new List<JobPost>
            {
                new JobPost { JobPostId = 10, Title = "Job A", EmployerId = employerId, Employer = new User { UserId = employerId, FullName = "Nhà tuyển dụng A" } }
            };
            _jobPostRepoMock.Setup(r => r.GetByEmployerIdAsync(employerId)).ReturnsAsync(posts);

            // Hành động
            var result = await _service.GetByEmployerIdAsync(employerId);

            // Kiểm tra
            Assert.Single(result);
            Assert.Equal(10, result.First().JobPostId);
        }

        [Fact]
        public async Task LayChiTietJobPostTheoIdTraVeDung()
        {
            // Sắp xếp
            int jobPostId = 1;
            var jobPostEntity = new JobPost { JobPostId = jobPostId, Title = "Chi tiết" };
            _jobPostRepoMock.Setup(r => r.GetJobPostByIdAsync(jobPostId)).ReturnsAsync(jobPostEntity);

            // Hành động
            var result = await _service.GetJobPostDetailByIdAsync(jobPostId);

            // Kiểm tra
            Assert.NotNull(result);
            Assert.Equal(jobPostId, result.JobPostId);
        }

        [Fact]
        public async Task LayJobPostPhanTrangTraVeDung()
        {
            // Sắp xếp
            int page = 1, pageSize = 10;
            var posts = new List<JobPost>
            {
                new JobPost { JobPostId = 1, Title = "Job 1" },
                new JobPost { JobPostId = 2, Title = "Job 2" }
            };
            int totalItems = 2;
            _jobPostRepoMock.Setup(r => r.GetPagedJobPostsAsync(page, pageSize, null, null)).ReturnsAsync((posts, totalItems));

            // Hành động
            var (result, total) = await _service.GetPagedJobPostsAsync(page, pageSize);

            // Kiểm tra
            Assert.Equal(2, result.Count());
            Assert.Equal(totalItems, total);
        }

        [Fact]
        public async Task LayJobPostTheoLocTraVeDung()
        {
            // Sắp xếp
            int page = 1, pageSize = 10, provinceId = 1;
            var posts = new List<JobPost>
            {
                new JobPost { JobPostId = 1, Title = "Job 1" },
                new JobPost { JobPostId = 2, Title = "Job 2" }
            };
            int totalItems = 2;
            _jobPostRepoMock.Setup(r => r.GetFilteredJobPostsAsync(page, pageSize, provinceId, null, null, null, null, null, null, null, null, null)).ReturnsAsync((posts, totalItems));

            // Hành động
            var (result, total) = await _service.GetFilteredJobPostsAsync(page, pageSize, provinceId);

            // Kiểm tra
            Assert.Equal(2, result.Count());
            Assert.Equal(totalItems, total);
        }

        [Fact]
        public async Task ThemJobPostMoiTraVeDung()
        {
            // Sắp xếp
            var dto = new AddJobPostDTO { Title = "Job mới", EndDate = DateTime.Now, Description = "Mô tả", WorkLocation = "Hà Nội", Status = "Active" };
            int employerId = 1;
            var jobPostEntity = new JobPost { JobPostId = 1, Title = "Job mới" };
            _jobPostRepoMock.Setup(r => r.AddJobPostAsync(It.IsAny<JobPost>())).ReturnsAsync(jobPostEntity);
            _jobPostRepoMock.Setup(r => r.GetJobPostByIdAsync(1)).ReturnsAsync(jobPostEntity);

            // Hành động
            var result = await _service.AddJobPostAsync(dto, employerId);

            // Kiểm tra
            Assert.NotNull(result);
            Assert.Equal("Job mới", result.Title);
        }

        [Fact]
        public async Task CapNhatJobPostTraVeDung()
        {
            // Sắp xếp
            var dto = new UpdateJobPostDTO { JobPostId = 1, Title = "Job update", EndDate = DateTime.Now, Description = "Mô tả update", WorkLocation = "HCM", Status = "Active" };
            int employerId = 1;
            var jobPostEntity = new JobPost { JobPostId = 1, Title = "Job update", EmployerId = employerId };
            _jobPostRepoMock.Setup(r => r.GetJobPostByIdAsync(1)).ReturnsAsync(jobPostEntity);
            _jobPostRepoMock.Setup(r => r.UpdateJobPostAsync(It.IsAny<JobPost>())).ReturnsAsync(jobPostEntity);

            // Hành động
            var result = await _service.UpdateJobPostAsync(dto, employerId);

            // Kiểm tra
            Assert.NotNull(result);
            Assert.Equal("Job update", result.Title);
        }

        [Fact]
        public async Task LayJobPostTheoCompanyIdTraVeDung()
        {
            // Sắp xếp
            int companyId = 1, page = 1, pageSize = 10;
            var posts = new List<JobPost>
            {
                new JobPost { JobPostId = 1, Title = "Job 1" },
                new JobPost { JobPostId = 2, Title = "Job 2" }
            };
            int totalItems = 2;
            _jobPostRepoMock.Setup(r => r.GetJobPostsByCompanyIdAsync(companyId, page, pageSize)).ReturnsAsync((posts, totalItems));

            // Hành động
            var (result, total) = await _service.GetJobPostsByCompanyIdAsync(companyId, page, pageSize);

            // Kiểm tra
            Assert.Equal(2, result.Count());
            Assert.Equal(totalItems, total);
        }

        [Fact]
        public async Task LayCvSubmissionsTheoJobPostTraVeDung()
        {
            // Sắp xếp
            int jobPostId = 1;
            var submissions = new List<Cvsubmission>
            {
                new Cvsubmission { SubmissionId = 1, SubmittedByUser = new User { FullName = "Ứng viên A" }, Cv = new Cv { FileUrl = "urlA" }, Status = "Đã nộp" },
                new Cvsubmission { SubmissionId = 2, SubmittedByUser = new User { FullName = "Ứng viên B" }, Cv = new Cv { FileUrl = "urlB" }, Status = "Đã nộp" }
            };
            _jobPostRepoMock.Setup(r => r.GetCvSubmissionsByJobPostIdAsync(jobPostId)).ReturnsAsync(submissions);

            // Hành động
            var result = await _service.GetCvSubmissionsByJobPostIdAsync(jobPostId);

            // Kiểm tra
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.CandidateName == "Ứng viên A");
            Assert.Contains(result, s => s.CandidateName == "Ứng viên B");
        }
    }
}
