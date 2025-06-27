using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository
{
    public class AccountListRepository : IAccountListRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public AccountListRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountDTOForList>> GetAllAccountsAsync()
        {
            var accounts = from a in _context.Accounts
                           join r in _context.Roles on a.RoleId equals r.RoleId
                           join u in _context.Users on a.AccountId equals u.AccountId
                           where a.RoleId == 2 || a.RoleId == 3
                           select new AccountDTOForList
                           {
                               AccountId = a.AccountId,
                               Email = a.Email,
                               RoleName = r.RoleName,
                               FullName = u.FullName,
                               Address = u.Address,
                               CreatedDate = a.CreatedDate,
                               Status = a.IsActive == true ? "Active" : "Inactive"

                           };

            return await accounts.ToListAsync();
        }

        public async Task<IEnumerable<AccountDTOForList>> GetAccountsByRoleAsync(string roleName)
        {
            var accounts = from a in _context.Accounts
                           join r in _context.Roles on a.RoleId equals r.RoleId
                           join u in _context.Users on a.AccountId equals u.AccountId
                           where r.RoleName == roleName && (a.RoleId == 2 || a.RoleId == 3)
                           select new AccountDTOForList
                           {
                               AccountId = a.AccountId,
                               Email = a.Email,
                               RoleName = r.RoleName,
                               FullName = u.FullName,
                               Address = u.Address,
                               CreatedDate = a.CreatedDate,
                               Status = a.IsActive == true ? "Active" : "Inactive"
                           };

            return await accounts.ToListAsync();
        }
    }
}
