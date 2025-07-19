using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository
{
    public class UserDetailOfAdminRepository : IUserDetailOfAdminRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public UserDetailOfAdminRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<UserDetailOfAdminDTO> GetUserDetailByAccountIdAsync(int accountId)
        {
            var query = from user in _context.Users
                        join acc in _context.Accounts on user.AccountId equals acc.AccountId
                        join company in _context.Companies on user.CompanyId equals company.CompanyId into comp
                        from company in comp.DefaultIfEmpty()
                        where user.AccountId == accountId
                        select new UserDetailOfAdminDTO
                        {
                            UserId = user.UserId,
                            FullName = user.FullName,
                            Phone = user.Phone,
                            Avatar = user.Avatar,
                            DOB = user.Dob,
                            Gender = user.Gender,
                            Address = user.Address,
                            LinkedIn = user.LinkedIn,
                            Facebook = user.Facebook,
                            CreatedDate = user.CreatedDate,
                            IsActive = user.IsActive,

                            AccountEmail = acc.Email,
                            CompanyName = company.CompanyName,
                            IsBan = user.IsBan,
                            AccountId = acc.AccountId
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}
