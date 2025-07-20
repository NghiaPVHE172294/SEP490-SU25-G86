using SEP490_SU25_G86_API.Models;
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

        public async Task<bool> SaveJobAsync(int userId, int jobPostId)
        {
            var existing = await _savedJobRepo.GetByUserAndJobPostAsync(userId, jobPostId);
            if (existing != null) return false; // đã lưu

            var newSaved = new SavedJob
            {
                UserId = userId,
                JobPostId = jobPostId,
                SaveAt = DateTime.UtcNow
            };

            await _savedJobRepo.CreateAsync(newSaved);
            return true;
        }

        public async Task<bool> IsJobSavedAsync(int userId, int jobPostId)
        {
            var saved = await _savedJobRepo.GetByUserAndJobPostAsync(userId, jobPostId);
            return saved != null;
        }

        public async Task<bool> DeleteSavedJobAsync(int saveJobId)
        {
            return await _savedJobRepo.DeleteAsync(saveJobId);
        }
    }


}
