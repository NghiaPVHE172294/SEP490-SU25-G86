using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RolePermissionRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.RolePermissionService
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _repository;

        public RolePermissionService(IRolePermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RolePermission>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<RolePermission?> GetByIdAsync(int roleId, int permissionId) =>
            await _repository.GetByIdAsync(roleId, permissionId);

        public async Task AddAsync(RolePermission entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(RolePermission entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(int roleId, int permissionId) => await _repository.DeleteAsync(roleId, permissionId);
    }
}

