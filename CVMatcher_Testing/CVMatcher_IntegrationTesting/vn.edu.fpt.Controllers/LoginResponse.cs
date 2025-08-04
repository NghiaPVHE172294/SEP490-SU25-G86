namespace vn.edu.fpt.Controllers
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int? UserId { get; set; } // Thêm userId để lấy id ứng viên cho test
    }
}
