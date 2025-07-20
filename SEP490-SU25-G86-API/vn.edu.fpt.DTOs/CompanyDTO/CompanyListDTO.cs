namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO
{
    public class CompanyListDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string? Website { get; set; }
        public string CompanySize { get; set; }
        public string Phone { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string IndustryName { get; set; }
        public int FollowerCount { get; set; }

        public int TotalJobPostEnabled { get; set; }
    }
}
