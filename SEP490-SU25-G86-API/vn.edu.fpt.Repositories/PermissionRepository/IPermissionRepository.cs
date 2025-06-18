namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PermissionRepository
{
    public interface IPermissionRepository
    {
        Task<bool> HasPermissionAsync(int accountId, string endpoint, string method);
    }

}
