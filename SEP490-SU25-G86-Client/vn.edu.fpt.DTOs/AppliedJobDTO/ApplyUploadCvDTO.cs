using Microsoft.AspNetCore.Http;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AppliedJobDTO
{
    public class ApplyUploadCvDTO
    {
        public int JobPostId { get; set; }
        public int CandidateId { get; set; }
        public string? Notes { get; set; }
        public string? CVName { get; set; }
        public IFormFile File { get; set; }
    }
} 