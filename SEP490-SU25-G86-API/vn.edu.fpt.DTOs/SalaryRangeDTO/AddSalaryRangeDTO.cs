using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.SalaryRangeDTO
{
    public class AddSalaryRangeDTO
    {
        [Required]
        public int MinSalary { get; set; }
        [Required]
        public int MaxSalary { get; set; }
        [Required]
        [StringLength(10)]
        public string Currency { get; set; }
    }
}
