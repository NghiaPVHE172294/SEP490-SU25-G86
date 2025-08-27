using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO
{
    public class CompanyCreateUpdateDTO
    {
        [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên công ty không được vượt quá 200 ký tự.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã số thuế là bắt buộc.")]
        [RegularExpression(@"^\d{10}(-\d{3})?$", ErrorMessage = "Mã số thuế phải gồm 10 số hoặc 13 ký tự (10 số + '-XXX').")]
        public string TaxCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngành nghề là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn ngành nghề hợp lệ.")]
        public int IndustryId { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Website không được vượt quá 200 ký tự.")]
        [RegularExpression(@"^((http|https):\/\/)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Website không đúng định dạng.")]
        public string? Website { get; set; }

        [Required(ErrorMessage = "Quy mô công ty là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Quy mô không được vượt quá 100 ký tự.")]
        public string CompanySize { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        //[RegularExpression(@"^(0[3|5|7|8|9][0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (10 số, bắt đầu bằng 03,05,07,08,09).")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Logo URL không được vượt quá 300 ký tự.")]
        [Url(ErrorMessage = "Logo URL không đúng định dạng.")]
        public string? LogoUrl { get; set; }

        public IFormFile? LogoFile { get; set; }
    }
}
