using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO
{
    public class CvDTO
    {
        public int CvId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CVName { get; set; }
        public bool IsUsed { get; set; } // true nếu CV đã dùng để apply
   
    }
} 