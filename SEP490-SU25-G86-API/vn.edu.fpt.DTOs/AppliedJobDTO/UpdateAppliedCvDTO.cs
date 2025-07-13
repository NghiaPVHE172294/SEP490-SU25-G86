using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AppliedJobDTO
{
    public class UpdateAppliedCvDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SubmissionId phải lớn hơn 0")]
        public int SubmissionId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "NewCvId phải lớn hơn 0")]
        public int NewCvId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId phải lớn hơn 0")]
        public int UserId { get; set; }
    }
} 