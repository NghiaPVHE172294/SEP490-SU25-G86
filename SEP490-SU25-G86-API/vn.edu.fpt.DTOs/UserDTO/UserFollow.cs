namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO
{
    public class UserFollow
    {
        public class FollowRequest
        {
            public int UserId { get; set; }
            public int CompanyId { get; set; }
        }

        public class BlockRequest
        {
            public int UserId { get; set; }
            public int CompanyId { get; set; }
            public string? Reason { get; set; }
        }
    }
}
