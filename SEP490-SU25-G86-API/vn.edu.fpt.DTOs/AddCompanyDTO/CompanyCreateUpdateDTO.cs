using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO
{
    public class CompanyCreateUpdateDTO
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên công ty không vượt quá 100 ký tự")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã số thuế là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã số thuế không vượt quá 20 ký tự")]
        public string TaxCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngành nghề là bắt buộc")]
        public int IndustryId { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công ty là bắt buộc")]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "Website không hợp lệ")]
        public string? Website { get; set; }

        [Required(ErrorMessage = "Quy mô công ty là bắt buộc")]
        public string CompanySize { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Url(ErrorMessage = "Link logo không hợp lệ")]
        public string? LogoUrl { get; set; }
    }
}
