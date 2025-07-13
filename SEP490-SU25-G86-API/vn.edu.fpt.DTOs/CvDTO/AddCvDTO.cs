using Microsoft.AspNetCore.Http;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO
{
    public class AddCvDTO
    {
        public IFormFile File { get; set; } = null!;
        public string? Notes { get; set; }
        public string? CVName { get; set; }
    }
} 