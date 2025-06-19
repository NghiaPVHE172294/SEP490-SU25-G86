namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService
{
    public interface IPermissionService
    {
        Task<bool> CheckAccessAsync(int accountId, string endpoint, string method);
    }

}
