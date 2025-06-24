namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO
{
    public class AppliedJobDTO
    {
        public int SubmissionId { get; set; }
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string? WorkLocation { get; set; }
        public string? Status { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
} 