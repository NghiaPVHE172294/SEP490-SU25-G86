using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO
{
    public class JobPostListDTO
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Salary { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? JobLevel { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Industry { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
