using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository
{
    public class AccountListRepository : IAccountListRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public AccountListRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<AccountDTOForList>> GetAccountsAsync(
        string? roleName, string? name, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            // Base query: chỉ lấy Employer/Candidate
            var q =
                from a in _context.Accounts.AsNoTracking()
                join r in _context.Roles.AsNoTracking() on a.RoleId equals r.RoleId
                join u in _context.Users.AsNoTracking() on a.AccountId equals u.AccountId
                where a.RoleId == 2 || a.RoleId == 3
                select new { a, r, u };

            if (!string.IsNullOrWhiteSpace(roleName) && !roleName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                q = q.Where(x => x.r.RoleName == roleName);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var nameLower = name.ToLower();
                q = q.Where(x => x.u.FullName != null && x.u.FullName.ToLower().Contains(nameLower));
            }

            // Sắp xếp ổn định để phân trang
            q = q.OrderByDescending(x => x.a.CreatedDate).ThenBy(x => x.a.AccountId);

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AccountDTOForList
                {
                    AccountId = x.a.AccountId,
                    Email = x.a.Email,
                    RoleName = x.r.RoleName,
                    FullName = x.u.FullName,
                    Address = x.u.Address,
                    CreatedDate = x.a.CreatedDate,
                    Status = (bool)x.a.IsActive ? "Active" : "Inactive"
                })
                .ToListAsync(ct);

            return new PagedResponse<AccountDTOForList>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
