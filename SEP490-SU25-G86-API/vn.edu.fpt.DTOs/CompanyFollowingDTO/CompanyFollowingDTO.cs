namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO
{
    public class CompanyFollowingDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string IndustryName { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Description { get; set; }
        public DateTime FlowedAt { get; set; }
        public string? Location { get; set; }
        public int ActiveJobsCount { get; set; }
        public int FollowId { get; set; }
    }
}
