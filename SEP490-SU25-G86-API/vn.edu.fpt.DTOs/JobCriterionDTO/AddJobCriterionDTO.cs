using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO
{
    public class AddJobCriterionDTO
    {
        public int JobPostId { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải nằm trong khoảng từ 0 đến 50 năm.")]
        public double? RequiredExperience { get; set; }

        [Required(ErrorMessage = "Kỹ năng yêu cầu không được để trống.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Kỹ năng yêu cầu phải từ 2 đến 500 ký tự.")]
        [MaxLength(500)]
        public string RequiredSkills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trình độ học vấn không được để trống.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Trình độ học vấn phải từ 2 đến 100 ký tự.")]
        [MaxLength(100)]
        public string EducationLevel { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Trình độ học vấn phải từ 2 đến 100 ký tự.")]
        [MaxLength(200)]
        public string? RequiredJobTitles { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Ngôn ngữ ưu tiên phải từ 2 đến 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredLanguages { get; set; }

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Chứng chỉ ưu tiên phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? PreferredCertifications { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Địa chỉ phải từ 2 đến 300 ký tự.")]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Tóm tắt phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Summary { get; set; }

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Lịch sử công việc phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? WorkHistory { get; set; }

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Dự án phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Projects { get; set; }
    }

    public class UpdateJobCriterionDTO
    {
        public int JobCriteriaId { get; set; }
        public int JobPostId { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải nằm trong khoảng từ 0 đến 50 năm.")]
        public double? RequiredExperience { get; set; }

        [Required(ErrorMessage = "Kỹ năng yêu cầu không được để trống.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Kỹ năng yêu cầu phải từ 2 đến 500 ký tự.")]
        [MaxLength(500)]
        public string RequiredSkills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trình độ học vấn không được để trống.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Trình độ học vấn phải từ 2 đến 100 ký tự.")]
        [MaxLength(100)]
        public string EducationLevel { get; set; } = string.Empty;

        [StringLength(200, MinimumLength = 2,   ErrorMessage = "Trình độ học vấn phải từ 2 đến 200 ký tự.")]
        [MaxLength(200)]
        public string? RequiredJobTitles { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Ngôn ngữ ưu tiên phải từ 2 đến 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredLanguages { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Chứng chỉ ưu tiên phải từ 2 đến 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredCertifications { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Địa chỉ phải từ 2 đến 300 ký tự.")]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Tóm tắt phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Summary { get; set; }

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Kinh nghiệm làm việc phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? WorkHistory { get; set; }

        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Dự án phải từ 2 đến 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Projects { get; set; }
    }
}
