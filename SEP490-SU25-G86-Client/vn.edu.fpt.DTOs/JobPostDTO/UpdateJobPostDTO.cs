using System;
using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO
{
    public class UpdateJobPostDTO
    {
        public int? CvtemplateOfEmployerId { get; set; } // ID của CVTemplate do employer chọn
        [Required]
        public int JobPostId { get; set; }
        public int? IndustryId { get; set; }
        public string? NewIndustryName { get; set; }
        public int? JobPositionId { get; set; }
        public string? NewJobPositionName { get; set; }
        public int? SalaryRangeId { get; set; }
        public string? NewSalaryRange { get; set; } // "min-max-currency"
        public int? ProvinceId { get; set; }
        public string? NewProvinceName { get; set; }
        public int? ExperienceLevelId { get; set; }
        public string? NewExperienceLevelName { get; set; }
        public int? JobLevelId { get; set; }
        public string? NewJobLevelName { get; set; }
        public int? EmploymentTypeId { get; set; }
        public string? NewEmploymentTypeName { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string? Description { get; set; }

        [StringLength(2000)]
        public string? CandidaterRequirements { get; set; }
        [StringLength(2000)]
        public string? Interest { get; set; }

        [Required]
        [StringLength(255)]
        public string WorkLocation { get; set; }

        public bool IsAienabled { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
} 