using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO
{
    public class AddJobCriterionDTO
    {
        [Required(ErrorMessage = "JobPostId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "JobPostId phải lớn hơn 0.")]
        public int JobPostId { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải nằm trong khoảng từ 0 đến 50 năm.")]
        public double? RequiredExperience { get; set; }

        [Required(ErrorMessage = "Kỹ năng yêu cầu không được để trống.")]
        [StringLength(500, ErrorMessage = "Kỹ năng yêu cầu không được vượt quá 500 ký tự.")]
        [MaxLength(500)]
        public string RequiredSkills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trình độ học vấn không được để trống.")]
        [StringLength(100, ErrorMessage = "Trình độ học vấn không được vượt quá 100 ký tự.")]
        [MaxLength(100)]
        public string EducationLevel { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Chức danh công việc yêu cầu không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? RequiredJobTitles { get; set; }

        [StringLength(200, ErrorMessage = "Ngôn ngữ ưu tiên không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredLanguages { get; set; }

        [StringLength(200, ErrorMessage = "Chứng chỉ ưu tiên không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredCertifications { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Tóm tắt không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Summary { get; set; }

        [StringLength(1000, ErrorMessage = "Kinh nghiệm làm việc không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? WorkHistory { get; set; }

        [StringLength(1000, ErrorMessage = "Dự án không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Projects { get; set; }
    }

    public class UpdateJobCriterionDTO
    {
        [Required(ErrorMessage = "JobCriteriaId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "JobCriteriaId phải lớn hơn 0.")]
        public int JobCriteriaId { get; set; }

        [Required(ErrorMessage = "JobPostId là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "JobPostId phải lớn hơn 0.")]
        public int JobPostId { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải nằm trong khoảng từ 0 đến 50 năm.")]
        public double? RequiredExperience { get; set; }

        [Required(ErrorMessage = "Kỹ năng yêu cầu không được để trống.")]
        [StringLength(500, ErrorMessage = "Kỹ năng yêu cầu không được vượt quá 500 ký tự.")]
        [MaxLength(500)]
        public string RequiredSkills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trình độ học vấn không được để trống.")]
        [StringLength(100, ErrorMessage = "Trình độ học vấn không được vượt quá 100 ký tự.")]
        [MaxLength(100)]
        public string EducationLevel { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Chức danh công việc yêu cầu không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? RequiredJobTitles { get; set; }

        [StringLength(200, ErrorMessage = "Ngôn ngữ ưu tiên không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredLanguages { get; set; }

        [StringLength(200, ErrorMessage = "Chứng chỉ ưu tiên không được vượt quá 200 ký tự.")]
        [MaxLength(200)]
        public string? PreferredCertifications { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Tóm tắt không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Summary { get; set; }

        [StringLength(1000, ErrorMessage = "Kinh nghiệm làm việc không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? WorkHistory { get; set; }

        [StringLength(1000, ErrorMessage = "Dự án không được vượt quá 1000 ký tự.")]
        [MaxLength(1000)]
        public string? Projects { get; set; }
    }
}
