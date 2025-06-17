using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService
{
    public interface ISavedJobService
    {
        Task<IEnumerable<SavedJobDTO>> GetSavedJobsByUserIdAsync(int userId);
    }
}
