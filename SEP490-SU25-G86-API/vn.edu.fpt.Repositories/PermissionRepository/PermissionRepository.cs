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

        private bool MatchEndpoint(string pattern, string actual)
        {
            var patternSegments = pattern.Trim('/').Split('/');
            var actualSegments = actual.Trim('/').Split('/');

            if (patternSegments.Length != actualSegments.Length)
                return false;

            for (int i = 0; i < patternSegments.Length; i++)
            {
                if (patternSegments[i].StartsWith("{") && patternSegments[i].EndsWith("}"))
                    continue; // Skip dynamic segments
                if (!string.Equals(patternSegments[i], actualSegments[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        public async Task<bool> HasPermissionAsync(int accountId, string endpoint, string method)
        {
            //var account = await _context.Accounts
            //    .Include(a => a.Role)
            //        .ThenInclude(r => r.Permissions)
            //    .FirstOrDefaultAsync(a => a.AccountId == accountId);

            //if (account == null || account.Role == null)
            //    return false;

            //return account.Role.Permissions.Any(p =>
            //    MatchEndpoint(p.Endpoint, endpoint) &&
            //    p.Method.ToUpper() == method.ToUpper());

            var account = await _context.Accounts
                .Include(a => a.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null || account.Role == null)
                return false;

            return account.Role.RolePermissions.Any(rp =>
                rp.IsAuthorized == true &&
                MatchEndpoint(rp.Permission.Endpoint, endpoint) &&
                rp.Permission.Method.ToUpper() == method.ToUpper());
        }
    }
}
