using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO
{
    public class JobCriterionDTO
    {
        public int JobCriteriaId { get; set; }
        public int JobPostId { get; set; }
        public double? RequiredExperience { get; set; }
        public string? RequiredSkills { get; set; }
        public string? EducationLevel { get; set; }
        public string? RequiredJobTitles { get; set; }
        public string? PreferredLanguages { get; set; }
        public string? PreferredCertifications { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
        public string? WorkHistory { get; set; }
        public string? Projects { get; set; }
        public string? Availability { get; set; }
        public string? SalaryExpectation { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 