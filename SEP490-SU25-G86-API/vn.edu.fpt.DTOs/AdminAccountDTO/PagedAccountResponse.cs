using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO
{
    public class PagedAccountResponse
    {
        public List<AccountDTOForList> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
