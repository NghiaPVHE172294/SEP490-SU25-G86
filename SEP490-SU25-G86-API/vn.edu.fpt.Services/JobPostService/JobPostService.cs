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
    }

}
