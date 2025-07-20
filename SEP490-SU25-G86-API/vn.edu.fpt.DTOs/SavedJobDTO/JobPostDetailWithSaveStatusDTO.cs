using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.SavedJobDTO
{
    public class JobPostDetailWithSaveStatusDTO
    {
        public ViewDetailJobPostDTO JobPostDetail { get; set; }
        public bool IsSaved { get; set; }
    }
}
