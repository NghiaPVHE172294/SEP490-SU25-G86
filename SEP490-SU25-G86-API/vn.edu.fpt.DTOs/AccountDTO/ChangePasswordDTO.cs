namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AccountDTO
{
    public class ChangePasswordDTO
    {
        public int AccountId { get; set; }
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
