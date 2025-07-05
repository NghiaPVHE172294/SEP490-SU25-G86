using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO
{
    public class AddCompanyDTO
    {
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(500)]
        public string? TaxCode { get; set; }
    }
}
