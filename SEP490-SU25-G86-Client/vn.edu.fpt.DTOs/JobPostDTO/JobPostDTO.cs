namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO
{
    public class JobPostDTO
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string? WorkLocation { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
