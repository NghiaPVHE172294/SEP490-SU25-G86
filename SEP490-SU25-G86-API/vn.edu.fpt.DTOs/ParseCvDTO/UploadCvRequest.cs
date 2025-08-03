using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.ParseCvDTO
{
    public class UploadCvRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
