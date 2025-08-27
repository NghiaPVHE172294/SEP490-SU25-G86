using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO
{
    public class ViewDetailJobPostDTO
    {
        public int JobPostId { get; set; }
        public int? IndustryId { get; set; }
        public int? JobPositionId { get; set; }
        public string? Title { get; set; }
        public int? SalaryRangeId { get; set; }
        public int? ProvinceId { get; set; }
        public int? ExperienceLevelId { get; set; }
        public int? JobLevelId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? CandidaterRequirements { get; set; }
        public string? Interest { get; set; }
        public string? WorkLocation { get; set; }
        public bool IsAienabled { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? EmployerId { get; set; }
        public string? EmployerName { get; set; } // Tên công ty
        public string? IndustryName { get; set; }
        public string? JobPositionName { get; set; }
        public string? SalaryRangeName { get; set; }
        public string? ProvinceName { get; set; }
        public string? ExperienceLevelName { get; set; }
        public string? JobLevelName { get; set; }
        public string? EmploymentTypeName { get; set; }
        // Company liên kết với JobPost
        public string? CompanyName { get; set; }
        public string? LogoUrl { get; set; }
        public string CompanySize { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Website { get; set; }
        // CVTemplate liên kết với JobPost
        public int? CvTemplateId { get; set; }
        public string? CvTemplateName { get; set; }
        public string? DocFileUrl { get; set; }
        public string? PdfFileUrl { get; set; }
        //bo sung saved job
        public bool IsSaved { get; set; }
        public int? CurrentUserId { get; set; }

    }
} 