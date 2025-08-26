using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO
{
    public class UpdateJobPostDTO : IValidatableObject
    {
        public int? CvtemplateOfEmployerId { get; set; } // ID của CVTemplate do employer chọn

        [Required(ErrorMessage = "JobPostId là bắt buộc.")]
        public int JobPostId { get; set; }

        public int? IndustryId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên ngành phải từ 2-255 ký tự.")]
        public string? NewIndustryName { get; set; }

        public int? JobPositionId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên vị trí phải từ 2-255 ký tự.")]
        public string? NewJobPositionName { get; set; }

        public int? SalaryRangeId { get; set; }
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Khoảng lương phải từ 5-255 ký tự.")]
        public string? NewSalaryRange { get; set; } // "min-max-currency"

        public int? ProvinceId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên tỉnh/thành phố phải từ 2-255 ký tự.")]
        public string? NewProvinceName { get; set; }

        public int? ExperienceLevelId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên cấp độ kinh nghiệm phải từ 2-255 ký tự.")]
        public string? NewExperienceLevelName { get; set; }

        public int? JobLevelId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên cấp bậc công việc phải từ 2-255 ký tự.")]
        public string? NewJobLevelName { get; set; }

        public int? EmploymentTypeId { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên loại hình làm việc phải từ 2-255 ký tự.")]
        public string? NewEmploymentTypeName { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Tiêu đề phải từ 3-255 ký tự.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc.")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mô tả phải từ 10-2000 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu ứng viên là bắt buộc.")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Yêu cầu ứng viên phải từ 5-2000 ký tự.")]
        public string? CandidaterRequirements { get; set; }

        [Required(ErrorMessage = "Quyền lợi là bắt buộc.")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Quyền lợi phải từ 5-2000 ký tự.")]
        public string? Interest { get; set; }

        [Required(ErrorMessage = "Địa điểm làm việc là bắt buộc.")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Địa điểm làm việc phải từ 2-255 ký tự.")]
        public string WorkLocation { get; set; }

        public bool IsAienabled { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public string Status { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            // Rule: EndDate phải cách hiện tại ít nhất 7 ngày
            if (EndDate.HasValue && EndDate.Value.Date < DateTime.UtcNow.Date.AddDays(7))
            {
                errors.Add(new ValidationResult(
                    "EndDate phải cách ngày hiện tại ít nhất 7 ngày.",
                    new[] { nameof(EndDate) }));
            }
            // Validate khi nhập NewSalaryRange
            if (!string.IsNullOrWhiteSpace(NewSalaryRange))
            {
                var parts = NewSalaryRange.Split('-');
                if (parts.Length < 3 ||
                    !decimal.TryParse(parts[0], out var minSalary) ||
                    !decimal.TryParse(parts[1], out var maxSalary) ||
                    minSalary <= 0 || maxSalary <= 0 || minSalary > maxSalary ||
                    string.IsNullOrWhiteSpace(parts[2]))
                {
                    errors.Add(new ValidationResult(
                        "NewSalaryRange phải đúng định dạng 'min-max-currency' và min/max > 0.",
                        new[] { nameof(NewSalaryRange) }));
                }
            }
            return errors;
        }
    }
}
