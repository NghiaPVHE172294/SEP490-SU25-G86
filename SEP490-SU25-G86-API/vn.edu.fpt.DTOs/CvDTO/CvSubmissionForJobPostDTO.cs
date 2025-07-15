namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO
{
    public class CvSubmissionForJobPostDTO
    {
        public int SubmissionId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string CandidateName { get; set; } = null!;
        public string CvFileUrl { get; set; } = null!;
        public string? Status { get; set; } // Đổi từ IsShortlisted sang Status
        public double? TotalScore { get; set; } // Thêm trường TotalScore
        public string? RecruiterNote { get; set; }
    }
} 