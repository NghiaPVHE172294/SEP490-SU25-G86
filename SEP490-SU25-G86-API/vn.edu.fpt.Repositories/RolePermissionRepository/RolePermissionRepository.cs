using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RolePermissionRepository
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public RolePermissionRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RolePermission?> GetByIdAsync(int roleId, int permissionId)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public async Task AddAsync(RolePermission entity)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = entity.RoleId,
                PermissionId = entity.PermissionId,
                IsAuthorized = entity.IsAuthorized
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RolePermission entity)
        {
            var existing = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == entity.RoleId && rp.PermissionId == entity.PermissionId);

            if (existing != null)
            {
                existing.IsAuthorized = entity.IsAuthorized;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int roleId, int permissionId)
        {
            var entity = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (entity != null)
            {
                _context.RolePermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
