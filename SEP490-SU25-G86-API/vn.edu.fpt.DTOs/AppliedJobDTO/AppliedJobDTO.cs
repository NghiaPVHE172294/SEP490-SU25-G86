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
        public int? CvId { get; set; }
        public string? CvName { get; set; }
        public string? CvFileUrl { get; set; }
        public string? CvNotes { get; set; }
        public string? SourceType { get; set; }
        public bool? IsDelete { get; set; }
        // --- Thêm cho UI mới ---
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string? Salary { get; set; }
    }
} 