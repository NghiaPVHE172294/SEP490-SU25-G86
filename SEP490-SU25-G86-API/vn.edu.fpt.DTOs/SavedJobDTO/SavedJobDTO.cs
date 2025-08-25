namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO
{
    public class SavedJobDTO
    {
        public int SaveJobId { get; set; }
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string? WorkLocation { get; set; }
        public string? Status { get; set; }
        public DateTime? SaveAt { get; set; }

        // --- Thêm cho UI mới ---
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public string? Salary { get; set; }
    }
}
