namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.LoginDTO
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}