using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService
{
    public class SavedJobService : ISavedJobService
    {
        private readonly ISavedJobRepository _savedJobRepo;

        public SavedJobService(ISavedJobRepository savedJobRepo)
        {
            _savedJobRepo = savedJobRepo;
        }

        public async Task<IEnumerable<SavedJobDTO>> GetSavedJobsByUserIdAsync(int userId)
        {
            var savedJobs = await _savedJobRepo.GetByUserIdAsync(userId);

            return savedJobs.Select(s => new SavedJobDTO
            {
                SaveJobId = s.SaveJobId,
                JobPostId = s.JobPostId,
                Title = s.JobPost?.Title ?? "",
                WorkLocation = s.JobPost?.WorkLocation,
                Status = s.JobPost?.Status,
                SaveAt = s.SaveAt
            });
        }
    }
}
