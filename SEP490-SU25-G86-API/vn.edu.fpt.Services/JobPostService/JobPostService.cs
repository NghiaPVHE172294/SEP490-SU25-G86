using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;


namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jobPostRepo;

        public JobPostService(IJobPostRepository jobPostRepo)
        {
            _jobPostRepo = jobPostRepo;
        }
        
        public async Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null)
        {
            var (posts, totalItems) = await _jobPostRepo.GetPagedJobPostsAsync(page, pageSize, region);

            var result = posts.Select(j => new JobPostHomeDto
            {
                JobPostId = j.JobPostId,
                Title = j.Title,
                CompanyName = j.Employer?.FullName ?? "Không rõ",
                Salary = j.SalaryRange != null
                         ? $"{j.SalaryRange.MinSalary:N0} - {j.SalaryRange.MaxSalary:N0} {j.SalaryRange.Currency}"
                         : "Thỏa thuận",
                Location = j.Province?.ProvinceName ?? "Không xác định"
            }).ToArray();

            return (result, totalItems);
        }

        public async Task<IEnumerable<JobPostDTO>> GetAllJobPostsAsync()
        {
            var posts = await _jobPostRepo.GetAllAsync();
            return posts.Select(post => new JobPostDTO
            {
                JobPostId = post.JobPostId,
                Title = post.Title,
                WorkLocation = post.WorkLocation,
                Status = post.Status,
                CreatedDate = post.CreatedDate,
                EndDate = post.EndDate
            });
        }

        public async Task<IEnumerable<JobPostDTO>> GetByEmployerIdAsync(int employerId)
        {
            var posts = await _jobPostRepo.GetByEmployerIdAsync(employerId);
            return posts.Select(post => new JobPostDTO
            {
                JobPostId = post.JobPostId,
                Title = post.Title,
                WorkLocation = post.WorkLocation,
                Status = post.Status,
                CreatedDate = post.CreatedDate,
                EndDate = post.EndDate
            });
        }

    }

}
