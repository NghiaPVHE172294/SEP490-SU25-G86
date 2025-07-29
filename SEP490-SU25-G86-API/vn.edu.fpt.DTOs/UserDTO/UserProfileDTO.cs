namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string? Avatar { get; set; }
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Dob { get; set; }
        public string? LinkedIn { get; set; }
        public string? Facebook { get; set; }
        public string? AboutMe { get; set; }
    }
}
