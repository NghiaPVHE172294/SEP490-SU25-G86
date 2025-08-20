namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO
{
    public class JobPostHomeDto
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public int? CompanyId { get; set; }
        public string Salary { get; set; } = null!;
        public string Location { get; set; } = null!;
        public bool IsApplied { get; set; }
        public string CompanyLogoUrl { get; set; } = null!;
    }
}
