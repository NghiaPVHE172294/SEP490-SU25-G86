namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Website { get; set; }
        public string CompanySize { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string IndustryName { get; set; } = null!;
        public int FollowersCount { get; set; }
    }
}
