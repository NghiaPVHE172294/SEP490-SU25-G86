using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PermissionRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<bool> CheckAccessAsync(int accountId, string endpoint, string method)
        {
            return await _permissionRepository.HasPermissionAsync(accountId, endpoint, method);
        }
    }

}
