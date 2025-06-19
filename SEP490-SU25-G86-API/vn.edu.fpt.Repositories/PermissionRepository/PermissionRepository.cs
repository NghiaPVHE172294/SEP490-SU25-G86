using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PermissionRepository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public PermissionRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(int accountId, string endpoint, string method)
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null || account.Role == null)
                return false;

            return account.Role.Permissions.Any(p =>
                p.Endpoint.ToLower() == endpoint.ToLower() &&
                p.Method.ToUpper() == method.ToUpper());

        }
    }
}
