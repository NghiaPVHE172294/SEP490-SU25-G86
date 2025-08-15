namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO
{
    public class UserDetailOfAdminDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string LinkedIn { get; set; }
        public string Facebook { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }

        public string AccountEmail { get; set; }
        public string CompanyName { get; set; }
        public bool IsBan { get; set; }
        public int AccountId { get; set; }
    }
}
