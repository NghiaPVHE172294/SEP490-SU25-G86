using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO
{
    public class JobPostListDTO
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public int? CompanyId { get; set; }
        public string CompanyLogoUrl { get; set; } = null!;
        public string? Salary { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? JobLevel { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Industry { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? WorkLocation { get; set; }
        public bool IsApplied { get; set; }
        public string DaysSincePostedOrUpdated
        {
            get
            {
                var days = UpdatedDate.HasValue
                    ? (DateTime.UtcNow.Date - UpdatedDate.Value.Date).Days
                    : (DateTime.UtcNow.Date - CreatedDate!.Value.Date).Days;
                return days == 0
                    ? (UpdatedDate.HasValue ? "Cập nhật hôm nay" : "Đăng hôm nay")
                    : (UpdatedDate.HasValue ? $"Cập nhật {days} ngày trước" : $"Đăng {days} ngày trước");
            }
        }
    }
}
