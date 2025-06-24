using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices
{
    public class AppliedJobService : IAppliedJobService
    {
        private readonly IAppliedJobRepository _appliedJobRepo;

        public AppliedJobService(IAppliedJobRepository appliedJobRepo)
        {
            _appliedJobRepo = appliedJobRepo;
        }

        public async Task<IEnumerable<AppliedJobDTO>> GetAppliedJobsByUserIdAsync(int userId)
        {
            var submissions = await _appliedJobRepo.GetByUserIdAsync(userId);
            return submissions.Select(s => new AppliedJobDTO
            {
                SubmissionId = s.SubmissionId,
                JobPostId = s.JobPostId ?? 0,
                Title = s.JobPost?.Title ?? string.Empty,
                WorkLocation = s.JobPost?.WorkLocation,
                Status = s.JobPost?.Status,
                SubmissionDate = s.SubmissionDate
            });
        }
    }
} 