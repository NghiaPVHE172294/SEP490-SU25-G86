namespace vn.edu.fpt.Controllers
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        // Add other fields if needed based on AuthController's actual response
    }
}
