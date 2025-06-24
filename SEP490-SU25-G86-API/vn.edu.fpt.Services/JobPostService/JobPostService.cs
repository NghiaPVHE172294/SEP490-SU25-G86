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

        public async Task<ViewDetailJobPostDTO?> GetJobPostDetailByIdAsync(int jobPostId)
        {
            var jobPost = await _jobPostRepo.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return null;
            return new ViewDetailJobPostDTO
            {
                JobPostId = jobPost.JobPostId,
                IndustryId = jobPost.IndustryId,
                JobPositionId = jobPost.JobPositionId,
                Title = jobPost.Title,
                SalaryRangeId = jobPost.SalaryRangeId,
                ProvinceId = jobPost.ProvinceId,
                ExperienceLevelId = jobPost.ExperienceLevelId,
                JobLevelId = jobPost.JobLevelId,
                EmploymentTypeId = jobPost.EmploymentTypeId,
                EndDate = jobPost.EndDate,
                Description = jobPost.Description,
                CandidaterRequirements = jobPost.CandidaterRequirements,
                Interest = jobPost.Interest,
                WorkLocation = jobPost.WorkLocation,
                IsAienabled = jobPost.IsAienabled,
                Status = jobPost.Status,
                CreatedDate = jobPost.CreatedDate,
                UpdatedDate = jobPost.UpdatedDate,
                EmployerId = jobPost.EmployerId,
                EmployerName = jobPost.Employer?.FullName,
                IndustryName = jobPost.Industry?.IndustryName,
                JobPositionName = jobPost.JobPosition?.PostitionName,
                SalaryRangeName = jobPost.SalaryRange != null ? $"{jobPost.SalaryRange.MinSalary:N0} - {jobPost.SalaryRange.MaxSalary:N0} {jobPost.SalaryRange.Currency}" : null,
                ProvinceName = jobPost.Province?.ProvinceName,
                ExperienceLevelName = jobPost.ExperienceLevel?.ExperienceLevelName,
                JobLevelName = jobPost.JobLevel?.JobLevelName,
                EmploymentTypeName = jobPost.EmploymentType?.EmploymentTypeName,
                CompanyName = jobPost.Employer?.Company?.CompanyName
            };
        }

    }

}
