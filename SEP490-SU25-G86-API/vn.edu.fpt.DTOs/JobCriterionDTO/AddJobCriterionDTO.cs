using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO
{
    public class AddJobCriterionDTO
    {
        [Required]
        public int JobPostId { get; set; }
        public double? RequiredExperience { get; set; }
        public string? RequiredSkills { get; set; }
        public string? EducationLevel { get; set; }
        public string? RequiredJobTitles { get; set; }
        public string? PreferredLanguages { get; set; }
        public string? PreferredCertifications { get; set; }
    }

    public class UpdateJobCriterionDTO
    {
        [Required]
        public int JobCriteriaId { get; set; }
        [Required]
        public int JobPostId { get; set; }
        public double? RequiredExperience { get; set; }
        public string? RequiredSkills { get; set; }
        public string? EducationLevel { get; set; }
        public string? RequiredJobTitles { get; set; }
        public string? PreferredLanguages { get; set; }
        public string? PreferredCertifications { get; set; }
    }
} 