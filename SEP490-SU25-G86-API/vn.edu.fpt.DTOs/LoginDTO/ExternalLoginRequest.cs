namespace SEP490_SU25_G86_API.vn.edu.fpt.DTO.LoginDTO
{
    public class ExternalLoginRequest
    {
        public string Provider { get; set; } // "Google"
        public string IdToken { get; set; } // Google ID Token
    }
}