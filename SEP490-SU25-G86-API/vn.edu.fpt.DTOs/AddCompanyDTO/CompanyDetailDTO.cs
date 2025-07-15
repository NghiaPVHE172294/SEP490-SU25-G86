namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO
{
    public class CompanyDetailDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string IndustryName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string CompanySize { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int IndustryId { get; set; }

        public string? LogoUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
