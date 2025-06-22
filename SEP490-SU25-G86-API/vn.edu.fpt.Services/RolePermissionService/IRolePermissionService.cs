using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.RolePermissionService
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission?> GetByIdAsync(int roleId, int permissionId);
        Task AddAsync(RolePermission entity);
        Task UpdateAsync(RolePermission entity);
        Task DeleteAsync(int roleId, int permissionId);
    }
}
