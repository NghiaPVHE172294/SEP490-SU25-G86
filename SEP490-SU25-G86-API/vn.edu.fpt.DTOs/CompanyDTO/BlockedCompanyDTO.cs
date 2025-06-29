namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO
{
    public class BlockedCompanyDTO
    {
        public int BlockedCompaniesId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? LogoUrl { get; set; }
        public string? Reason { get; set; }
    }
} 