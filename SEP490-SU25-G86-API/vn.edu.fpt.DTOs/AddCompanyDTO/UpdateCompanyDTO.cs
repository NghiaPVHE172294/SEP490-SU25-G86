namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO
{
    public class UpdateCompanyDTO
    {
        public string? CompanyName { get; set; }
        public string? Website { get; set; }
        public string? CompanySize { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public int? IndustryId { get; set; }
        public IFormFile? LogoFile { get; set; }
    }
}
